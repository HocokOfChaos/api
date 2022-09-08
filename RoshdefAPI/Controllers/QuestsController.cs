using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Attributes.Routing;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Request;
using RoshdefAPI.Models.Response;
using RoshdefAPI.Shared.Services.Core;
using System.Net.Mime;
using static RoshdefAPI.Data.Models.Config;

namespace RoshdefAPI.Controllers
{
    [Produces(MediaTypeNames.Application.Json), ApiController, APIRoute("[controller]")]
    public class QuestsController : APIController
    {
        private readonly ILogger<QuestsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlayersRepositoryBase _playerRepository;
        private readonly IMapper _mapper;
        private readonly MatchesRepositoryBase _matchRepository;
        private readonly MatchesPlayersRepositoryBase _matchPlayerRepository;
        private readonly IQuestsService _questsService;
        private readonly ConfigRepositoryBase _configRepository;

        public QuestsController(
            ILogger<QuestsController> logger, 
            IUnitOfWork uow, 
            PlayersRepositoryBase playerRepository, 
            IMapper mapper, 
            MatchesRepositoryBase matchRepository, 
            MatchesPlayersRepositoryBase matchPlayerRepository, 
            IQuestsService questsService, 
            ConfigRepositoryBase configRepository
        ) : base(logger)
        {
            _logger = logger;
            _unitOfWork = uow;
            _playerRepository = playerRepository;
            _mapper = mapper;
            _matchRepository = matchRepository;
            _matchPlayerRepository = matchPlayerRepository;
            _questsService = questsService;
            _configRepository = configRepository;
        }

        [HttpPost, Route("FinishDailyQuest")]
        public async Task<FinishDailyQuestResponse> FinishDailyQuest(FinishDailyQuestRequest request)
        {
            var response = new FinishDailyQuestResponse { Success = false };
            var steamID = request.Data.SteamID;
            var crystals = request.Data.Crystals;
            var soulStones = request.Data.SoulStones;
            var coins = request.Data.Coins;
            var matchID = request.Data.MatchID;
            if (crystals < 0)
            {
                return Error(ref response, $"Attempt to finish daily quest with invalid crystals reward specified ({nameof(request.Data.Crystals)} = {crystals}), positive integer expected.");
            }
            if (soulStones < 0)
            {
                return Error(ref response, $"Attempt to finish daily quest with invalid soul stones reward specified ({nameof(request.Data.SoulStones)} = {soulStones}), positive integer expected.");
            }
            if (coins < 0)
            {
                return Error(ref response, $"Attempt to finish daily quest with invalid coins reward specified ({nameof(request.Data.Coins)} = {coins}), positive integer expected.");
            }
            var player = await _playerRepository.FindBySteamID(steamID, false);
            if (player == null)
            {
                return Error(ref response, $"Attempt to finish daily quest for non registered player ({nameof(request.Data.SteamID)} = {steamID}).");
            }
            if (player.IsDailyQuestAvailable == false)
            {
                return Error(ref response, $"Attempt to finish daily quest for player that already finished it today. ({nameof(request.Data.SteamID)} = {steamID}).");
            }
            var match = await _matchRepository.FindByMatchID(matchID, true);
            if (match == null)
            {
                _logger.LogError($"Can't find match by id to record player match achievment ({nameof(request.Data.MatchID)} = {matchID})");
            }
            else
            {
                var matchPlayer = match.Players.FirstOrDefault(x => x.SteamID.Equals(steamID));
                if (matchPlayer == null)
                {
                    _logger.LogError($"Can't find player in match {matchID} ({nameof(request.Data.SteamID)} = {steamID})");
                }
                else
                {
                    matchPlayer.AddCoins(coins);
                    matchPlayer.AddCrystals(crystals);
                    matchPlayer.AddSoulStones(soulStones);
                    await _matchPlayerRepository.Update(matchPlayer);
                }
            }
            player.AddBalance(crystals, coins, soulStones);
            player.LastDailyQuestDateTime = DateTime.Now.Date;
            await _playerRepository.Update(player);
            response.Success = true;
            response.Data = new FinishDailyQuestResponseData
            {
                Balance = _mapper.Map<PlayerBalanceDTO>(player)
            };
            _unitOfWork.Commit();
            return response;
        }

        [HttpPost, Route("UpdateDailyQuest")]
        public async Task<BaseResponse> UpdateDailyQuest(BaseRequest request)
        {
            var response = new BaseResponse { Success = false };
            var dailyQuests = _questsService.GetAllDailyQuests();
            Random random = new();
            var randomQuestIndex = random.Next(0, dailyQuests.Count());
            var randomQuest = dailyQuests.ElementAt(randomQuestIndex);
            var result = await _configRepository.SetValue(ConfigType.DailyQuestID, randomQuest.ID.ToString());
            if (result == false)
            {
                return Error(ref response, $"Can't set new daily quest id, config table \"{ConfigType.DailyQuestID}\" entry is missing.");
            }
            response.Success = result;
            _unitOfWork.Commit();
            return response;
        }
    }
}
