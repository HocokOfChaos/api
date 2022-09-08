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
    public class LeaderboardController : APIController
    {
        private readonly ILogger<LeaderboardController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LeaderboardPlayersRepositoryBase _leaderboardPlayerRepository;
        private readonly PlayersRepositoryBase _playerRepositoryBase;
        private readonly IMapper _mapper;

        public LeaderboardController(
            ILogger<LeaderboardController> logger, 
            IUnitOfWork unitOfWork, 
            LeaderboardPlayersRepositoryBase leaderboardPlayerRepositoryBase, 
            PlayersRepositoryBase playerRepositoryBase, 
            IMapper mapper
        ) : base(logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _leaderboardPlayerRepository = leaderboardPlayerRepositoryBase;
            _playerRepositoryBase = playerRepositoryBase;
            _mapper = mapper;
        }

        [HttpPost, Route("GetData")]
        public async Task<LeaderboardGetDataResponse> GetData(LeaderboardGetDataRequest request)
        {
            var response = new LeaderboardGetDataResponse();
            var playersInLeaderboard = request.Data.PlayersInLeaderboard;
            if (playersInLeaderboard < 1)
            {
                return Error(ref response, "Players in leaderboard must be positive integer greater than 0.");
            }
            List<LeaderboardPlayer> leaderboards = (await _leaderboardPlayerRepository.FindAll()).ToList();
            var missingPlayersAmount = playersInLeaderboard - leaderboards.Count;
            if (missingPlayersAmount > 0)
            {
                var missingPlayers = await _playerRepositoryBase.FindLastPlayers(missingPlayersAmount, false);
                foreach (var player in missingPlayers)
                {
                    leaderboards.Add(_mapper.Map<LeaderboardPlayer>(player));
                }
            }
            response.Success = true;
            response.Data = new Dictionary<ulong, LeaderboardGetDataResponseData>();
            foreach(var player in request.Data.Players)
            {
                var entry = new LeaderboardGetDataResponseData();
                var localPlayer = leaderboards.Where(playerInLeaderboard => playerInLeaderboard.SteamID.Equals(player.SteamID)).FirstOrDefault();
                if (localPlayer is null)
                {
                    leaderboards.Add(new LeaderboardPlayer(player.SteamID));
                }
                var localPlayerSteamID = player.SteamID.ToString();
                entry.Month = GenerateLeaderboard(leaderboards, localPlayerSteamID, playersInLeaderboard, x => x.MonthPoints);
                entry.Week = GenerateLeaderboard(leaderboards, localPlayerSteamID, playersInLeaderboard, x => x.WeekPoints);
                entry.Day = GenerateLeaderboard(leaderboards, localPlayerSteamID, playersInLeaderboard, x => x.DayPoints);
                if(!response.Data.TryAdd(player.SteamID, entry))
                {
                    _logger.LogError($"Attempt to add player with {nameof(player.SteamID)} = {player.SteamID} more than once to leaderboard response.");
                }
            }
            return response;
        }

        [HttpPost, Route("ResetDailyLeaderboard")]
        public async Task<ResetDailyLeaderboardResponse> ResetDailyLeaderboard(ResetDailyLeaderboardRequest request)
        {
            var response = new ResetDailyLeaderboardResponse { Success = true };
            await _leaderboardPlayerRepository.ResetDailyLeaderboard();
            _unitOfWork.Commit();
            return response;
        }

        [HttpPost, Route("ResetWeeklyLeaderboard")]
        public async Task<ResetWeeklyLeaderboardResponse> ResetWeeklyLeaderboard(ResetWeeklyLeaderboardRequest request)
        {
            var response = new ResetWeeklyLeaderboardResponse { Success = true };
            await _leaderboardPlayerRepository.ResetWeeklyLeaderboard();
            _unitOfWork.Commit();
            return response;
        }

        [HttpPost, Route("ResetMontlyLeaderboard")]
        public async Task<ResetMontlyLeaderboardResponse> ResetWeeklyLeaderboard(ResetMontlyLeaderboardRequest request)
        {
            var response = new ResetMontlyLeaderboardResponse { Success = true };
            await _leaderboardPlayerRepository.ResetMonthlyLeaderboard();
            _unitOfWork.Commit();
            return response;
        }

        private List<LeaderboardPlayerDTO> GenerateLeaderboard(IEnumerable<LeaderboardPlayer> leaderboards, string localPlayerSteamID, int playersLimit, Func<LeaderboardPlayer, ulong> selector)
        {
            List<LeaderboardPlayerDTO> result = new List<LeaderboardPlayerDTO>();
            var leaderboard = leaderboards.OrderByDescending(selector).Select((entry, index) =>
            {
                return _mapper.Map<LeaderboardPlayerDTO>(entry, opt =>
                {
                    opt.SetPoints(selector(entry));
                    opt.SetPlace(Convert.ToUInt32(index + 1));
                });
            });
            var localPlayer = leaderboard.Where(player => player.SteamID.Equals(localPlayerSteamID)).FirstOrDefault();
            leaderboard = leaderboard.Where(player =>
            {
                return player.Place <= playersLimit;
            });
            result.AddRange(leaderboard);
            if (localPlayer is null)
            {
                _logger.LogError($"Can't find player by {nameof(Player.SteamID)} = {localPlayerSteamID}.");
                result.Add(new LeaderboardPlayerDTO(localPlayerSteamID, Convert.ToUInt32(playersLimit + 1), 0));
            }
            else
            {
                if (localPlayer.Place > playersLimit)
                {
                    result.Add(localPlayer);
                }
            }
            return result;
        }
    }
}
