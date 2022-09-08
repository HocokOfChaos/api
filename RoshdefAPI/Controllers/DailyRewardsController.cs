using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Attributes.Routing;
using RoshdefAPI.AutoMapper.Extensions;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Request;
using RoshdefAPI.Models.Response;
using System.Net.Mime;

namespace RoshdefAPI.Controllers
{
    [Produces(MediaTypeNames.Application.Json), ApiController, APIRoute("[controller]")]
    public class DailyRewardsController : APIController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlayersRepositoryBase _playerRepository;
        private readonly PlayersItemsRepositoryBase _playerItemsRepository;
        private readonly DailyRewardsRepositoryBase _dailyRewardRepository;
        private readonly IMapper _mapper;

        public DailyRewardsController(
            ILogger<DailyRewardsController> logger, 
            IUnitOfWork uow, 
            PlayersRepositoryBase playerRepository, 
            PlayersItemsRepositoryBase playerItemsRepository, 
            DailyRewardsRepositoryBase dailyRewardRepository, 
            IMapper mapper
        ) : base(logger)
        {
            _playerRepository = playerRepository;
            _playerItemsRepository = playerItemsRepository;
            _dailyRewardRepository = dailyRewardRepository;
            _unitOfWork = uow;
            _mapper = mapper;
        }

        [HttpPost, Route("ConfirmDailyReward")]
        public async Task<DailyRewardConfirmResponse> ConfirmDailyReward(DailyRewardConfirmRequest request)
        {
            var response = new DailyRewardConfirmResponse { Success = false };
            var steamID = request.Data.SteamID;
            var player = await _playerRepository.FindBySteamID(steamID, true);

            if (player == null)
            {
                return Error(ref response, $"Attempt to confirm daily reward for non registered player (SteamID = {steamID}).");
            }
            if (steamID == 0)
            {
                return Error(ref response, $"Attempt to confirm daily reward for invalid player (SteamID = {steamID}).");
            }
            if (player.IsDailyRewardAvailable == false)
            {
                return Error(ref response, $"Attempt to confirm daily reward for player that already claimed it today (SteamID = {steamID}).");
            }
            var nextDailyRewardDay = player.CurrentDailyRewardDay + 1;
            var nextDailyReward = await _dailyRewardRepository.FindByDayNumber(nextDailyRewardDay);
            if (nextDailyReward == null)
            {
                return Error(ref response, $"Attempt to get daily reward for invalid day (Day = {nextDailyRewardDay}).");
            }
            var itemsForInsertion = new List<PlayerItem>();
            var itemsForUpdating = new List<PlayerItem>();
            foreach (var dailyRewardItem in nextDailyReward.Items)
            {
                var newItem = _mapper.Map<PlayerItem>(dailyRewardItem, options => options.SetDailyRewardPlayerID(player.ID));
                TimeSpan? newItemDuration = dailyRewardItem.Duration.HasValue ? TimeSpan.FromSeconds(dailyRewardItem.Duration.Value) : null;
                (var isAdded, var addedItem) = player.AddItem(newItem, newItemDuration);
                if (isAdded)
                {
                    itemsForInsertion.Add(addedItem);
                }
                else
                {
                    itemsForUpdating.Add(addedItem);
                }
            }
            await _playerItemsRepository.BulkInsert(itemsForInsertion, 25);
            await _playerItemsRepository.BulkUpdate(itemsForUpdating, 25);
            player.AddBalance(nextDailyReward.Crystals, nextDailyReward.Coins, nextDailyReward.SoulStones);
            player.LastDailyRewardDateTime = DateTime.Now.Date;
            player.CurrentDailyRewardDay = nextDailyRewardDay;
            await _playerRepository.Update(player);
            _unitOfWork.Commit();
            var playerDataDTO = _mapper.Map<PlayerDTO>(player);
            response.Success = true;
            response.Data = new DailyRewardConfirmResponseData();
            response.Data.Balance = playerDataDTO.Balance;
            response.Data.Items = playerDataDTO.Items;
            return response;
        }
    }
}
