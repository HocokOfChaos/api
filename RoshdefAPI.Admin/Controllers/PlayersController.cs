using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Admin.Models.Request;
using RoshdefAPI.Admin.Services.Core;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Shared.Services.Core;
using System.Net.Mime;

namespace RoshdefAPI.Admin.Controllers
{
    [Authorize]
    public class PlayersController : BaseController
    {
        private readonly PlayersRepositoryBase _playerRepository;
        private readonly PlayersItemsRepositoryBase _playersItemsRepository;
        private readonly PlayersLogsRepositoryBase _playersLogsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShopItemsService _shopItemsService;
        private readonly IJsonStringLocalizer _localizer;
        private readonly UserManager<User> _userManager;

        public PlayersController(
            ILogger<PlayersController> logger,
            PlayersRepositoryBase playerRepository,
            PlayersItemsRepositoryBase playersItemsRepository,
            PlayersLogsRepositoryBase playersLogsRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IShopItemsService shopItemsService,
            IJsonStringLocalizer localizer,
            UserManager<User> userManager
            ) : base(logger)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _playersItemsRepository = playersItemsRepository;
            _playersLogsRepository = playersLogsRepository;
            _unitOfWork = unitOfWork;
            _shopItemsService = shopItemsService;
            _localizer = localizer;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerDataResponse> GetData([FromForm] PlayerData data)
        {
            var response = new PlayerDataResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, true);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            response.Success = true;
            response.Data = _mapper.Map<PlayerDTO>(player);
            return response;
        }


        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerLogsResponse> GetLogs([FromForm] PlayerData data)
        {
            var response = new PlayerLogsResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, true);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var logs = await _playersLogsRepository.FindLogsBySteamID(data.SteamID);
            var result = new List<PlayerLogDTO>();
            foreach(var log in logs)
            {
                result.Add(_mapper.Map<PlayerLogDTO>(log));
            }
            response.Success = true;
            response.Data = result;
            return response;
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerItemDataResponse> UpdateItem([FromForm] PlayerItemData data)
        {
            var response = new PlayerItemDataResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            if (data.Count < 1)
            {
                return Error(ref response, _localizer["PlayersPage.Error.ItemCountMustBeGreaterThan0"]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, true);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var playerItem = player.Items.FirstOrDefault(x => x.ItemID.Equals(data.ItemID));
            if (playerItem == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.ItemNotExists"]);
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _playersLogsRepository.Log(
                data.SteamID,
                new AdminPanelUpdateItemLogEntry(user.Login, playerItem.ItemID, playerItem.Count, data.Count, playerItem.ExpireDate, data.ExpireDate)
            );
            playerItem.ExpireDate = data.ExpireDate;
            playerItem.Count = data.Count;
            await _playersItemsRepository.Update(playerItem);
            _unitOfWork.Commit();
            response.Success = true;
            response.Data = _mapper.Map<PlayerDTO>(player).Items.FirstOrDefault(x => x.ItemID.Equals(data.ItemID.ToString()));
            return response;
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerItemDataResponse> AddItem([FromForm] PlayerItemData data)
        {
            var response = new PlayerItemDataResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            if (data.Count < 1)
            {
                return Error(ref response, _localizer["PlayersPage.Error.ItemCountMustBeGreaterThan0"]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, true);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var now = DateTime.Now;
            TimeSpan? newItemDuration = data.ExpireDate.HasValue ? (data.ExpireDate - now) : null;
            if (newItemDuration.HasValue && newItemDuration.Value.TotalSeconds <= 0)
            {
                return Error(ref response, _localizer["PlayersPage.Error.ExpireDateMustBeGreaterThanNow", now]);
            }
            var newPlayerItem = new PlayerItem(data.ItemID, player.ID);
            newPlayerItem.Count = data.Count;
            (var isAdded, var addedItem) = player.AddItem(newPlayerItem, newItemDuration);
            if (isAdded)
            {
                await _playersItemsRepository.Insert(addedItem);
            }
            else
            {
                await _playersItemsRepository.Update(addedItem);
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _playersLogsRepository.Log(
                data.SteamID,
                new AdminPanelAddItemLogEntry(user.Login, data.ItemID, data.Count, newItemDuration)
            );
            _unitOfWork.Commit();
            response.Success = true;
            response.Data = _mapper.Map<PlayerDTO>(player).Items.FirstOrDefault(x => x.ItemID.Equals(data.ItemID.ToString()));
            return response;
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerItemDataResponse> DeleteItem([FromForm] PlayerItemData data)
        {
            var response = new PlayerItemDataResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, true);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var playerItem = player.Items.FirstOrDefault(x => x.ItemID.Equals(data.ItemID));
            if(playerItem == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.ItemNotExists"]);
            }
            await _playersItemsRepository.Delete(playerItem);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _playersLogsRepository.Log(
                data.SteamID,
                new AdminPanelDeleteItemLogEntry(user.Login, playerItem.ItemID)
            );
            _unitOfWork.Commit();
            response.Success = true;
            response.Data = null;
            return response;
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<PlayerBalanceResponse> UpdateBalance([FromForm] PlayerBalanceData data)
        {
            var response = new PlayerBalanceResponse();
            if (!ModelState.IsValid)
            {
                return Error(ref response, _localizer["PlayersPage.Error.FailedToReadFormData"]);
            }
            if (data.SteamID == PlayerConstants.InvalidSteamID)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            var player = await _playerRepository.FindBySteamID(data.SteamID, false);
            if (player == null)
            {
                return Error(ref response, _localizer["PlayersPage.Error.InvalidSteamID", data.SteamID]);
            }
            if(data.Crystals < 0)
            {
                return Error(ref response, _localizer["PlayersPage.Error.CrystalsMustBePositiveNumber"]);
            }
            if (data.Coins < 0)
            {
                return Error(ref response, _localizer["PlayersPage.Error.CoinsMustBePositiveNumber"]);
            }
            if (data.SoulStones < 0)
            {
                return Error(ref response, _localizer["PlayersPage.Error.SoulStonesMustBePositiveNumber"]);
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _playersLogsRepository.Log(
                data.SteamID,
                new AdminPanelChangeCurrencyLogEntry(user.Login, player.Crystals, player.Coins, player.SoulStones, data.Crystals, data.Coins, data.SoulStones)
            );
            player.SetBalance(data.Crystals, data.Coins, data.SoulStones);
            await _playerRepository.Update(player);
            _unitOfWork.Commit();
            response.Success = true;
            response.Data = new PlayerBalanceResponseContent
            {
                Balance = _mapper.Map<PlayerBalanceDTO>(player),
                SteamID = player.SteamID.ToString()
            };
            return response;
        }
    }
}
