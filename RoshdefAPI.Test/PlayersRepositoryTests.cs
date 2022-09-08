using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Test.Services;

namespace RoshdefAPI.Test
{
    [TestFixture]
    public class PlayersRepositoryTests
    {
        IUnitOfWork _unitOfWork;
        IQueryBuilder _queryBuilder;
        PlayersRepositoryBase _playersRepository;

        private void RestoreRepositories()
        {
            _playersRepository = new PlayersRepository(_unitOfWork, _queryBuilder, new PlayersItemRepository(_unitOfWork, _queryBuilder));
        }

        [SetUp]
        public async Task Setup()
        {
            _unitOfWork = new UnitOfWork(new DatabaseConnectionProviderTestMySQL(null));
            _queryBuilder = new QueryBuilderMySQL();
            RestoreRepositories();
            var players = await _playersRepository.FindAll();
            await _playersRepository.BulkDelete(players);
            _unitOfWork.Commit();
            RestoreRepositories();
        }

        [Test]
        public async Task FindBySteamID_ShouldReturnPlayer_WhenPlayerExists()
        {
            ulong playerSteamID = 1;
            await _playersRepository.Insert(new Player(playerSteamID));
            _unitOfWork.Commit();
            RestoreRepositories();
            var player = await _playersRepository.FindBySteamID(playerSteamID, false);
            Assert.IsTrue(player is not null);
        }

        [Test]
        public async Task Find_ShouldReturnPlayer_WhenPlayerExists()
        {
            ulong playerSteamID = 1;
            var player = new Player(playerSteamID);
            await _playersRepository.Insert(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.Find(player.ID, false);
            Assert.IsTrue(result is not null);
        }

        [Test]
        public async Task FindLastPlayers_ShouldReturnTenPlayers_WhenPlayersExists()
        {
            var testPlayers = new List<Player>();
            for (ulong i = 0; i < 10; i++)
            {
                testPlayers.Add(new Player(i));
            }
            await _playersRepository.BulkInsert(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            var allPlayers = await _playersRepository.FindLastPlayers(10, false);
            Assert.IsTrue(allPlayers.Count() == testPlayers.Count && allPlayers.Where(player => player.SteamID >= 0 && player.SteamID <= 10).Count() == testPlayers.Count);
        }
    }
}