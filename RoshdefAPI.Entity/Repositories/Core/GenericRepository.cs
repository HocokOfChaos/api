using Dapper;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Entity.Services.Core;
using System.Data;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class GenericRepository
    {
        protected readonly IUnitOfWork _unitOfWork;
        public abstract string TableName { get; }

        public GenericRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.Register(this);
        }
    }

    public abstract class GenericRepository<T> : GenericRepository where T : IDataObject
    {
        protected readonly IQueryBuilder QueryBuilder;
        protected IDbConnection Connection { get; }
        protected IDbTransaction Transaction { get; }

        public GenericRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork)
        {
            QueryBuilder = queryBuilder;
            Connection = _unitOfWork.Connection;
            Transaction = _unitOfWork.Transaction;
        }

        public virtual async Task Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.ID = await Connection.QuerySingleAsync<ulong>(
                QueryBuilder.BuildInsertQuery<T>(this),
                param: entity,
                transaction: Transaction
            );
        }
        public virtual async Task<IEnumerable<T>> FindAll()
        {
            return await Connection.QueryAsync<T>(
                QueryBuilder.BuildSelectQuery<T>(this),
                param: null,
                transaction: Transaction
            );
        }
        public virtual async Task<T?> Find(ulong id)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IDataObject.ID), id);
            return await Connection.QueryFirstOrDefaultAsync<T>(
                QueryBuilder.BuildSelectQuery<T>(this, $"WHERE t1.{nameof(IDataObject.ID)} = @{nameof(IDataObject.ID)}"),
                param: parameters,
                transaction: Transaction
            );
        }
        public virtual async Task Delete(ulong id)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IDataObject.ID), id);
            var sql = QueryBuilder.BuildDeleteQuery<T>(this, $"WHERE t1.{nameof(IDataObject.ID)} = @{nameof(IDataObject.ID)}");
            await Connection.ExecuteAsync(
                sql,
                param: parameters,
                transaction: Transaction
            );
        }
        public virtual async Task Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await Delete(entity.ID);
        }
        public virtual async Task Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            await Connection.ExecuteAsync(
                QueryBuilder.BuildUpdateQuery<T>(this, $"WHERE t1.{nameof(IDataObject.ID)} = @{nameof(IDataObject.ID)}"),
                param: entity,
                transaction: Transaction
            ); ;
        }

        /// <param name="entities">List of entities for delete query</param>
        /// <param name="batchSize">Max amount of entities to send per single request (very high numbers may reduce perfomance or cause problems)</param>
        public virtual async Task BulkDelete(IEnumerable<T> entities, int batchSize = 1000)
        {
            var count = entities.Count();
            for (int i = 0; i < count; i = i + batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize);
                await Connection.ExecuteAsync(
                    sql: QueryBuilder.BuildBulkDeleteQuery(batch, this),
                    param: null,
                    transaction: Transaction
                );
            }
        }

        /// <param name="entities">List of entities for insert query</param>
        /// <param name="batchSize">Max amount of entities to send per single request (very high numbers may reduce perfomance or cause problems)</param>
        public virtual async Task BulkInsert(IEnumerable<T> entities, int batchSize = 1000)
        {
            var count = entities.Count();
            for (int i = 0; i < count; i = i + batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize);
                var firstInsertedID = await Connection.QuerySingleAsync<ulong>(
                    sql: QueryBuilder.BuildBulkInsertQuery(batch, this),
                    param: null,
                    transaction: Transaction
                );
                foreach (var entity in batch)
                {
                    entity.ID = firstInsertedID;
                    firstInsertedID++;
                }
            }
        }

        /// <param name="entities">List of entities for update query</param>
        /// <param name="batchSize">Max amount of entities to send per single request (very high numbers may reduce perfomance or cause problems)</param>
        public virtual async Task BulkUpdate(IEnumerable<T> entities, int batchSize = 1000)
        {
            var count = entities.Count();
            for (int i = 0; i < count; i = i + batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize);
                await Connection.ExecuteAsync(
                    sql: QueryBuilder.BuildBulkUpdateQuery(batch, this),
                    param: null,
                    transaction: Transaction
                );
            }
        }
    }
}
