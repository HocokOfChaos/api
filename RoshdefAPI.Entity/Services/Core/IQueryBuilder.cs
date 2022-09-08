using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Entity.Models;
using RoshdefAPI.Entity.Repositories.Core;

namespace RoshdefAPI.Entity.Services.Core
{
    public interface IQueryBuilder
    {
        public string BuildInsertQuery<T>(GenericRepository<T> repository) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query for SELECT.
        /// </summary>
        /// <param name="repository">Repository that using T data model.</param>
        /// <param name="whereCondition">Where condition sql (if required). Select table can be referenced as t1.</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildSelectQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query for UPDATE.
        /// </summary>
        /// <param name="repository">Repository that using T data model.</param>
        /// <param name="whereCondition">Where condition sql (if required). Update table can be referenced as t1.</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildUpdateQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query for DELETE.
        /// </summary>
        /// <param name="repository">Repository that using T data model.</param>
        /// <param name="whereCondition">Where condition sql (if required). Delete table can be referenced as t1.</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildDeleteQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query for MAX(column).
        /// </summary>
        /// <param name="repository">Repository that using T data model.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildMaxColumnValueQuery<T>(GenericRepository<T> repository, string columnName) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query for UPDATE with certain column instead of all.
        /// </summary>
        /// <param name="repository">Repository that using T data model.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="whereCondition">Where condition sql (if required). Update table can be referenced as t1.</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildUpdateFieldQuery<T>(GenericRepository<T> repository, string columnName, string? whereCondition = null) where T : IDataObject;
        /// <summary>
        /// Generates SQL Query with joined tables.
        /// </summary>
        /// <param name="table1">First table for join. Can be referenced as t1 in whereCondition.</param>
        /// <param name="table2">Second table for join. Can be referenced as t2 in whereCondition.</param>
        /// <param name="whereCondition">Where condition sql (if required)</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildJoinQuery<T1, T2, T3>(ComplexQueryTable<T1> table1, ComplexQueryTable<T2> table2, ComplexQueryTable<T3> table3, string? whereCondition = null)
            where T1 : IDataObject
            where T2 : IDataObject
            where T3 : IDataObject;
        /// <summary>
        /// Generates SQL Query with joined tables.
        /// </summary>
        /// <param name="table1">First table for join. Can be referenced as t1 in whereCondition.</param>
        /// <param name="table2">Second table for join. Can be referenced as t2 in whereCondition.</param>
        /// <param name="table3">Third table for join. Can be referenced as t3 in whereCondition.</param>
        /// <param name="whereCondition">Where condition sql (if required)</param>
        /// <returns>Generated SQL query.</returns>
        public string BuildJoinQuery<T1, T2>(ComplexQueryTable<T1> table1, ComplexQueryTable<T2> table2, string? whereCondition = null)
            where T1 : IDataObject
            where T2 : IDataObject;
        public string BuildBulkInsertQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject;
        public string BuildBulkDeleteQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject;
        public string BuildBulkUpdateQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject;
    }
}
