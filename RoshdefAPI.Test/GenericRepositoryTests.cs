using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Test.Services;

namespace RoshdefAPI.Test
{
    [TestFixture]
    public class GenericRepositoryTests
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
        public void Insert_ShouldNotThrow_WhenPlayerInserted()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await _playersRepository.Insert(new Player(1));
                _unitOfWork.Commit();
            });
        }

        [Test]
        public async Task BulkInsert_ShouldInsertEntities_WhenEntitiesGiven()
        {
            var testPlayers = new List<Player>
            {
                new Player(1),
                new Player(2),
                new Player(3),
                new Player(4),
                new Player(5)
            };
            await _playersRepository.BulkInsert(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.FindAll();
            Assert.IsTrue(result.Count() == testPlayers.Count && result.Where(player => player.SteamID >= 1 && player.SteamID <= 5).Count() == testPlayers.Count);
        }

        [Test]
        public async Task BulkUpdate_ShouldUpdateEntities_WhenEntitiesGiven()
        {
            var testPlayers = new List<Player>
            {
                new Player(1),
                new Player(2),
                new Player(3),
                new Player(4),
                new Player(5)
            };
            await _playersRepository.BulkInsert(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            foreach(var player in testPlayers)
            {
                player.AddSoulStones(5);
            }
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.FindAll();
            Assert.IsTrue(result.Select(player => player.SoulStones == 5).Count() == 5);
        }

        [Test]
        public async Task BulkDelete_ShouldDeleteEntities_WhenEntitiesGiven()
        {
            var testPlayers = new List<Player>
            {
                new Player(1),
                new Player(2),
                new Player(3),
                new Player(4),
                new Player(5)
            };
            await _playersRepository.BulkInsert(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            await _playersRepository.BulkDelete(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.FindAll();
            Assert.IsTrue(result.Count() == 0);
        }

        [Test]
        public async Task Update_ShouldUpdateEntity_WhenEntityGiven()
        {
            var player = new Player(1);
            await _playersRepository.Insert(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            player.AddSoulStones(5);
            await _playersRepository.Update(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.FindBySteamID(player.SteamID, false);
            Assert.IsTrue(result is not null && result.SoulStones == 5);
        }

        [Test]
        public async Task Delete_ShouldDeleteEntity_WhenEntityGiven()
        {
            var player = new Player(1);
            await _playersRepository.Insert(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            await _playersRepository.Delete(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            var players = await _playersRepository.FindAll();
            Assert.IsTrue(players.Count() == 0);
        }

        [Test]
        public async Task Find_ShouldReturnEntity_WhenEntityGiven()
        {
            var player = new Player(1);
            await _playersRepository.Insert(player);
            _unitOfWork.Commit();
            RestoreRepositories();
            var result = await _playersRepository.Find(player.ID);
            Assert.IsTrue(result is not null);
        }

        [Test]
        public async Task FindAll_ShouldReturnFiveEntities_WhenFiveEntitiesGiven()
        {
            var testPlayers = new List<Player>
            {
                new Player(1),
                new Player(2),
                new Player(3),
                new Player(4),
                new Player(5)
            };
            await _playersRepository.BulkInsert(testPlayers);
            _unitOfWork.Commit();
            RestoreRepositories();
            var allPlayers = await _playersRepository.FindAll();
            Assert.IsTrue(allPlayers.Count() == testPlayers.Count && allPlayers.Where(player => player.SteamID >= 1 && player.SteamID <= 5).Count() == testPlayers.Count);
        }
    }
}