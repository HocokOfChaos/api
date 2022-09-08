using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Attributes.Routing;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Data.Constants;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Models.DTO;
using RoshdefAPI.Models.Request;
using RoshdefAPI.Models.Response;
using RoshdefAPI.Shared.Models;
using RoshdefAPI.Shared.Services.Core;
using System.Net.Mime;
using System.Text.Json;
using static RoshdefAPI.Data.Models.Config;

namespace RoshdefAPI.Controllers
{
    [Produces(MediaTypeNames.Application.Json), ApiController, APIRoute("[controller]")]
    public class MatchController : APIController
    {
        private readonly ILogger<MatchController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MatchesRepositoryBase _matchRepository;
        private readonly MatchesPlayersRepositoryBase _matchPlayerRepository;
        private readonly PlayersRepositoryBase _playerRepository;
        private readonly PlayersItemsRepositoryBase _playerItemRepository;
        private readonly LeaderboardPlayersRepositoryBase _leaderboardPlayerRepository;
        private readonly IShopItemsService _shopItemsService;
        private readonly IMapper _mapper;
        private readonly PlayersItemsRepositoryBase _playerItemsRepository;
        private readonly DailyRewardsRepositoryBase _dailyRewardsRepository;
        private readonly ConfigRepositoryBase _configRepository;

        public MatchController(
            ILogger<MatchController> logger,
            IUnitOfWork uow,
            MatchesRepositoryBase matchRepository,
            MatchesPlayersRepositoryBase matchPlayerRepository,
            PlayersRepositoryBase playerRepository,
            PlayersItemsRepositoryBase playerItemRepository,
            LeaderboardPlayersRepositoryBase leaderboardPlayerRepository,
            IShopItemsService shopItemsService,
            IMapper mapper,
            PlayersItemsRepositoryBase playerItemsRepository,
            DailyRewardsRepositoryBase dailyRewardsRepository,
            ConfigRepositoryBase configRepository
        ) : base(logger)
        {
            _logger = logger;
            _unitOfWork = uow;
            _matchRepository = matchRepository;
            _matchPlayerRepository = matchPlayerRepository;
            _playerRepository = playerRepository;
            _playerItemRepository = playerItemRepository;
            _leaderboardPlayerRepository = leaderboardPlayerRepository;
            _shopItemsService = shopItemsService;
            _mapper = mapper;
            _playerItemsRepository = playerItemsRepository;
            _dailyRewardsRepository = dailyRewardsRepository;
            _configRepository = configRepository;
        }

        [HttpPost, Route("BeginMatch")]
        public async Task<BeginMatchResponse> BeginMatch(BeginMatchRequest request)
        {
            var response = new BeginMatchResponse
            {
                Success = true,
                Data = new BeginMatchResponseData()
            };
            var newMatch = await _matchRepository.FindByDOTAMatchID(request.Data.DotaMatchID, true);
            var lastDailyRewardDay = await _dailyRewardsRepository.FindLastDayNumber();
            var players = new List<Player>();
            var playersDTO = new List<PlayerDTO>();
            var isDataModified = false;
            if (newMatch == null)
            {
                newMatch = new Match(request.Data.DotaMatchID);
                await _matchRepository.Insert(newMatch);
                players = (await _playerRepository.FindAllBySteamIDs(request.Data.Players.Select(x => x.SteamID), true)).ToList();
                foreach (var playerInRequest in request.Data.Players)
                {
                    var player = players.FirstOrDefault(x => x.SteamID.Equals(playerInRequest.SteamID));
                    if (player == null)
                    {
                        player = new Player(playerInRequest.SteamID);
                        await _playerRepository.Insert(player);
                        players.Add(player);
                    }
                    await FindAndRemoveExpiredItems(player);
                    playersDTO.Add(_mapper.Map<Player, PlayerDTO>(player));
                    var matchPlayer = new MatchPlayer(player.SteamID, newMatch.ID);
                    await _matchPlayerRepository.Insert(matchPlayer);
                }
                isDataModified = true;
            }
            else
            {
                players = (await _playerRepository.FindAllBySteamIDs(request.Data.Players.Select(x => x.SteamID), true)).ToList();
                playersDTO = _mapper.Map<List<Player>, List<PlayerDTO>>(players);
            }
            playersDTO.ForEach(player =>
            {
                player.IsDailyRewardAvailable = player.IsDailyRewardAvailable && player.CurrentDailyRewardDay < lastDailyRewardDay;
            });
            var dailyQuest = await _configRepository.GetValue(ConfigType.DailyQuestID);
            if (dailyQuest == null)
            {
                _logger.LogError("Can't get current daily quest id, config table \"{DailyQuestID}\" entry is missing.", ConfigType.DailyQuestID);
                response.Data.DailyQuestID = QuestsConstants.InvalidDailyQuestID;
            }
            else
            {
                if (!uint.TryParse(dailyQuest.Value, out uint dailyQuestID))
                {
                    _logger.LogError("Can't get current daily quest id, config table \"{DailyQuestID}\" have invalid value (positive integer expected).", ConfigType.DailyQuestID);
                    response.Data.DailyQuestID = QuestsConstants.InvalidDailyQuestID;
                }
                else
                {
                    response.Data.DailyQuestID = dailyQuestID;
                }
            }
            response.Data.Players = playersDTO;
            response.Data.MatchID = newMatch.ID;
            response.Data.LastDailyRewardDay = lastDailyRewardDay;
            if (isDataModified)
            {
                _unitOfWork.Commit();
            }
            return response;
        }

        [HttpPost, Route("FinishMatch")]
        public async Task<FinishMatchResponse> FinishMatch(FinishMatchRequest request)
        {
            var matchID = request.Data.MatchID;
            var matchDuration = request.Data.Duration;
            var matchWinner = request.Data.Winner;
            var matchDifficulty = request.Data.Difficulty;
            var response = new FinishMatchResponse { Success = true };
            var match = await _matchRepository.FindByMatchID(matchID, true);
            if (match == null)
            {
                return Error(ref response, $"Can't find match by specified id ({matchID}).");
            }
            if (match.IsFinished())
            {
                response.Data = new FinishMatchResponseData
                {
                    Players = new Dictionary<ulong, PlayerBalanceDTO>()
                };
                foreach (var matchPlayer in match.Players)
                {
                    var playerBalanceChanges = _mapper.Map<PlayerBalanceDTO>(matchPlayer);
                    if (!response.Data.Players.TryAdd(matchPlayer.SteamID, playerBalanceChanges))
                    {
                        _logger.LogError("Attempt to add player {steamID} = {playerSteamID} to response data more than once.", nameof(matchPlayer.SteamID), matchPlayer.SteamID);
                    }
                }
                return response;
            }
            match.Duration = matchDuration;
            match.Difficulty = matchDifficulty;
            match.Winner = matchWinner;
            match.EndDateTime = DateTime.Now;
            response.Data = new FinishMatchResponseData
            {
                Players = new Dictionary<ulong, PlayerBalanceDTO>()
            };
            foreach (var matchPlayerData in request.Data.Players.Values)
            {
                var matchPlayer = match.Players.FirstOrDefault(x => x.SteamID.Equals(matchPlayerData.SteamID));
                if (matchPlayer == null)
                {
                    _logger.LogError("Can't find match player with {steamID} = {playerSteamID} in match {matchID}.", nameof(matchPlayerData.SteamID), matchPlayerData.SteamID, matchID);
                    continue;
                }
                matchPlayer.BossesKilled = matchPlayerData.BossesKilled;
                matchPlayer.CreepsKilled = matchPlayerData.CreepsKilled;
                matchPlayer.DamageDone = matchPlayerData.DamageDone;
                matchPlayer.DamageTaken = matchPlayerData.DamageTaken;
                matchPlayer.ExpiriencePerMinute = matchPlayerData.ExpiriencePerMinute;
                matchPlayer.GoldPerMinute = matchPlayerData.GoldPerMinute;
                matchPlayer.HealingDone = matchPlayerData.HealingDone;
                try
                {
                    if (matchPlayerData.ItemsBuild.Equals("[]"))
                    {
                        matchPlayer.SetItemsBuild(new Dictionary<string, string>());
                    }
                    else
                    {
                        var playerItemBuild = JsonSerializer.Deserialize<Dictionary<string, string>>(matchPlayerData.ItemsBuild);
                        if (playerItemBuild != null)
                        {
                            matchPlayer.SetItemsBuild(playerItemBuild);
                        }
                        else
                        {
                            throw new Exception("Player item build json is invalid.");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to deserialize player ({steamID} = {playerSteamID}) item build json", nameof(matchPlayerData.SteamID), matchPlayerData.ItemsBuild);
                    matchPlayer.SetItemsBuild(new Dictionary<string, string>());
                }
                matchPlayer.LifestealDone = matchPlayerData.LifestealDone;
                matchPlayer.Networth = matchPlayerData.Networth;
                try
                {
                    if (matchPlayerData.QuestsFinished.Equals("[]"))
                    {
                        matchPlayer.SetQuestsFinished(new Dictionary<string, string>());
                    }
                    else
                    {
                        var playerQuestsFinished = JsonSerializer.Deserialize<Dictionary<string, string>>(matchPlayerData.QuestsFinished);
                        if (playerQuestsFinished != null)
                        {
                            matchPlayer.SetQuestsFinished(playerQuestsFinished);
                        }
                        else
                        {
                            throw new Exception("Player quests finished json is invalid.");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to deserialize player ({steamID} = {playerSteamID}) quests finished json. ({json})", nameof(matchPlayerData.SteamID), matchPlayerData.SteamID, matchPlayerData.QuestsFinished);
                    matchPlayer.SetQuestsFinished(new Dictionary<string, string>());
                }
                var coinsFromMatch = 0;
                if (match.Difficulty > MatchConstants.ExplorationDifficulty)
                {
                    coinsFromMatch = Convert.ToInt32(match.Difficulty * (match.Duration / 60));
                }
                matchPlayer.AddCoins(coinsFromMatch);
                await _matchPlayerRepository.Update(matchPlayer);
                await AddPointsToPlayerInLeaderboard(matchPlayer.SteamID, match.Difficulty, match.Duration);
                var player = await _playerRepository.FindBySteamID(matchPlayerData.SteamID, true);
                if (player == null)
                {
                    _logger.LogError("Can't find player by {steamID} = {playerSteamID}.", nameof(matchPlayerData.SteamID), matchPlayerData.SteamID);
                }
                else
                {
                    player.AddCoins(coinsFromMatch);
                    List<PlayerItem> itemsForDeletion = new();
                    List<PlayerItem> itemsForUpdating = new();
                    foreach (var consumedItem in matchPlayerData.ConsumedItems)
                    {
                        var playerItem = player.Items.Where(x => x.ItemID.Equals(consumedItem.ItemID)).FirstOrDefault();
                        if (playerItem == null)
                        {
                            _logger.LogError("Can't find player item of player with {steamID} = {playerSteamID} ({itemID} = {playerItemID}).", nameof(matchPlayerData.SteamID), matchPlayerData.SteamID, nameof(consumedItem.ItemID), consumedItem.ItemID);
                        }
                        else
                        {
                            if (player.ConsumeItem(playerItem, consumedItem.Count))
                            {
                                itemsForDeletion.Add(playerItem);
                            }
                            else
                            {
                                itemsForUpdating.Add(playerItem);
                            }
                        }
                    }
                    foreach (var dressedItem in matchPlayerData.DressedItems)
                    {
                        var dressedItemType = ShopItemType.Invalid;
                        var dressedShopItem = _shopItemsService.GetItemByID(dressedItem.ItemID);
                        if (dressedShopItem is null)
                        {
                            _logger.LogError("Attempt to dress unknown item ({itemID} = {playerItemID}) for player with {steamID} = {playerSteamID}.", nameof(dressedItem.ItemID), dressedItem.ItemID, nameof(matchPlayerData.SteamID), matchPlayerData.SteamID);
                            continue;
                        }
                        else
                        {
                            dressedItemType = dressedShopItem.Type;
                            var playerItemsWithRequiredType = player.Items.Where(x =>
                            {
                                var playerShopItem = _shopItemsService.GetItemByID(x.ItemID);
                                if (playerShopItem is null)
                                {
                                    return false;
                                }
                                else
                                {
                                    return playerShopItem.Type.Equals(dressedItemType);
                                }
                            });
                            foreach (var playerItem in playerItemsWithRequiredType)
                            {
                                playerItem.IsDressed = playerItem.ItemID.Equals(dressedItem.ItemID);
                            }
                            itemsForUpdating.AddRange(playerItemsWithRequiredType);
                        }
                    }
                    await _playerItemRepository.BulkUpdate(itemsForUpdating, 25);
                    await _playerItemRepository.BulkDelete(itemsForDeletion, 25);
                    await _playerRepository.Update(player);
                }
                var playerBalanceChanges = _mapper.Map<PlayerBalanceDTO>(matchPlayer);
                if (!response.Data.Players.TryAdd(matchPlayer.SteamID, playerBalanceChanges))
                {
                    _logger.LogError("Attempt to add player {steamID} = {playerSteamID} to response data more than once.", nameof(matchPlayer.SteamID), matchPlayer.SteamID);
                }
            }
            await _matchRepository.Update(match);
            _unitOfWork.Commit();
            return response;
        }
        private async Task AddPointsToPlayerInLeaderboard(ulong steamID, ulong duration, ulong difficulty)
        {
            var points = duration * difficulty;
            var playerInLeaderboard = await _leaderboardPlayerRepository.FindBySteamID(steamID);
            var isAdded = false;
            if (playerInLeaderboard == null)
            {
                playerInLeaderboard = new LeaderboardPlayer(steamID);
                isAdded = true;
            }
            playerInLeaderboard.AddPoints(points);
            if (isAdded)
            {
                await _leaderboardPlayerRepository.Insert(playerInLeaderboard);
            }
            else
            {
                await _leaderboardPlayerRepository.Update(playerInLeaderboard);
            }
        }
        private async Task<bool> FindAndRemoveExpiredItems(Player playerData)
        {
            List<PlayerItem> itemsForDeletion = new();
            var result = false;
            var todayTime = DateTime.Now;
            foreach (var item in playerData.Items.Where(item => item.IsExpired(todayTime)))
            {
                itemsForDeletion.Add(item);
                result = true;
            }
            foreach(var item in itemsForDeletion)
            {
                playerData.ConsumeItem(item, item.Count);
            }
            if (itemsForDeletion.Count > 0)
            {
                await _playerItemsRepository.BulkDelete(itemsForDeletion, 25);
            }
            return result;
        }
    }
}
