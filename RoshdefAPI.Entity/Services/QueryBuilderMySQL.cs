using RoshdefAPI.Data.Attributes;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Entity.Helpers;
using RoshdefAPI.Entity.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace RoshdefAPI.Entity.Services
{
    public class QueryBuilderMySQL : IQueryBuilder
    {
        private class ModelDescription { }

        private class ModelDescription<T> : ModelDescription
        {
            public ReadOnlyCollection<string> Properties { get; set; } = new List<string>().AsReadOnly();

            private readonly ConcurrentDictionary<string, Func<T, object>> _propertyGetters = new();

            public object GetPropertyValueByName(T entity, string propertyName)
            {
                return _propertyGetters[propertyName](entity);
            }

            public void BuildGetAccessor(string propertyName, Type classType)
            {
                var propertyInfo = classType.GetProperty(propertyName);
                var getter = FastInvoke.BuildUntypedGetter<T>(propertyInfo);
                _propertyGetters.TryAdd(propertyName, getter);
            }
        }

        private readonly ConcurrentDictionary<string, ModelDescription> _modelDescriptionCache = new();

        public string BuildJoinQuery<T1, T2>(ComplexQueryTable<T1> table1, ComplexQueryTable<T2> table2, string? whereCondition = null)
            where T1 : IDataObject
            where T2 : IDataObject
        {
            var firstTableProperties = GetQueryParams<T1>();
            var secondTableProperties = GetQueryParams<T2>();
            var complexQuery = new StringBuilder($"SELECT ");
            for (int i = 0; i < firstTableProperties.Count; i++)
            {
                complexQuery.Append($"t1.{firstTableProperties[i]},");
            }
            for (int i = 0; i < secondTableProperties.Count; i++)
            {
                complexQuery.Append($"t2.{secondTableProperties[i]},");
            }
            complexQuery.Remove(complexQuery.Length - 1, 1);
            complexQuery.Append($" FROM {table1.TableName} t1");
            complexQuery.Append(GetJoinTableStringForComplexQuery("t2", table2));
            if (whereCondition != null)
            {
                complexQuery.Append($" {whereCondition}");
            }
            return complexQuery.ToString();
        }

        public string BuildJoinQuery<T1, T2, T3>(ComplexQueryTable<T1> table1, ComplexQueryTable<T2> table2, ComplexQueryTable<T3> table3, string? whereCondition = null)
            where T1 : IDataObject
            where T2 : IDataObject
            where T3 : IDataObject
        {
            var firstTableProperties = GetQueryParams<T1>();
            var secondTableProperties = GetQueryParams<T2>();
            var thirdTableProperties = GetQueryParams<T3>();
            var complexQuery = new StringBuilder($"SELECT ");
            for (int i = 0; i < firstTableProperties.Count; i++)
            {
                complexQuery.Append($"t1.{firstTableProperties[i]},");
            }
            for (int i = 0; i < secondTableProperties.Count; i++)
            {
                complexQuery.Append($"t2.{secondTableProperties[i]},");
            }
            for (int i = 0; i < thirdTableProperties.Count; i++)
            {
                complexQuery.Append($"t3.{thirdTableProperties[i]},");
            }
            complexQuery.Remove(complexQuery.Length - 1, 1);
            complexQuery.Append($" FROM {table1.TableName} t1");
            complexQuery.Append(GetJoinTableStringForComplexQuery("t2", table2));
            complexQuery.Append(GetJoinTableStringForComplexQuery("t3", table3));
            if (whereCondition != null)
            {
                complexQuery.Append($" {whereCondition}");
            }
            return complexQuery.ToString();
        }
        public string BuildDeleteQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject
        {
            var deleteQuery = new StringBuilder($"DELETE t1 FROM `{repository.TableName}` t1");

            if (whereCondition != null)
            {
                deleteQuery.Append($" {whereCondition}");
            }

            return deleteQuery.ToString();
        }
        public string BuildInsertQuery<T>(GenericRepository<T> repository) where T : IDataObject
        {
            var classProperties = GetQueryParams<T>();
            // fix for id = 0
            classProperties = classProperties.Where(param => !param.Equals(nameof(IDataObject.ID))).ToList().AsReadOnly();

            var insertQuery = new StringBuilder($"INSERT INTO `{repository.TableName}` ");

            insertQuery.Append('(');

            for (int i = 0; i < classProperties.Count; i++)
            {
                insertQuery.Append($"`{classProperties[i]}`,");
            }

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            for (int i = 0; i < classProperties.Count; i++)
            {
                insertQuery.Append($"@{classProperties[i]},");
            }

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append("); SELECT LAST_INSERT_ID();");

            return insertQuery.ToString();
        }
        public string BuildSelectQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject
        {
            var classProperties = GetQueryParams<T>();

            var selectQuery = new StringBuilder("SELECT ");

            for (int i = 0; i < classProperties.Count; i++)
            {
                selectQuery.Append($"t1.{classProperties[i]},");
            }

            selectQuery
                .Remove(selectQuery.Length - 1, 1)
                .Append($" FROM `{repository.TableName}` t1");
            if (whereCondition != null)
            {
                selectQuery.Append($" {whereCondition}");
            }
            return selectQuery.ToString();
        }

        public string BuildUpdateQuery<T>(GenericRepository<T> repository, string? whereCondition = null) where T : IDataObject
        {
            var classProperties = GetQueryParams<T>();

            var updateQuery = new StringBuilder($"UPDATE `{repository.TableName}` t1 SET ");

            for (int i = 0; i < classProperties.Count; i++)
            {
                updateQuery.Append($"t1.{classProperties[i]} = @{classProperties[i]},");
            }

            updateQuery.Remove(updateQuery.Length - 1, 1);
            if (whereCondition != null)
            {
                updateQuery.Append($" {whereCondition}");
            }
            return updateQuery.ToString();
        }

        public string BuildUpdateFieldQuery<T>(GenericRepository<T> repository, string columnName, string? whereCondition = null) where T : IDataObject
        {
            var updateQuery = new StringBuilder($"UPDATE `{repository.TableName}` t1 SET ");

            updateQuery.Append($"t1.{columnName} = @{columnName}");

            if (whereCondition != null)
            {
                updateQuery.Append($" {whereCondition}");
            }
            return updateQuery.ToString();
        }

        public string BuildMaxColumnValueQuery<T>(GenericRepository<T> repository, string columnName) where T : IDataObject
        {
            return $"SELECT MAX({columnName}) FROM `{repository.TableName}`;";
        }

        public string BuildBulkInsertQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject
        {
            var modelDescription = GetModelDescription<T>();
            var query = new StringBuilder($"INSERT INTO {repository.TableName} (");
            var queryParams = GetQueryParams<T>();
            // fix for id = 0
            queryParams = queryParams.Where(param => !param.Equals(nameof(IDataObject.ID))).ToList().AsReadOnly();
            foreach (var queryParam in queryParams)
            {
                query.Append($"`{queryParam}`,");
            }
            query.Remove(query.Length - 1, 1);
            query.Append(") VALUES ");
            foreach (var entity in entities)
            {
                query.Append('(');
                foreach (var queryParam in queryParams)
                {
                    // Treat null as MySQL NULL instead of default type value
                    var value = modelDescription.GetPropertyValueByName(entity, queryParam);
                    if(value == null)
                    {
                        query.Append("NULL,");
                    } else
                    {
                        if(value is DateTime)
                        {
                            value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (value is bool)
                        {
                            value = ((bool)value) == true ? "1" : "0";
                        }
                        query.Append($"'{value}',");
                    }
                }
                query.Remove(query.Length - 1, 1);
                query.Append("),");
            }
            query.Remove(query.Length - 1, 1);
            // mysql returns first row id in case of multiple values insert
            query.Append("; SELECT LAST_INSERT_ID();");
            return query.ToString();
        }

        public string BuildBulkDeleteQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject
        {
            var query = new StringBuilder($"DELETE FROM  {repository.TableName} WHERE {nameof(IDataObject.ID)} IN (");
            foreach (var entity in entities)
            {
                query.Append($"'{entity.ID}',");
            }
            query.Remove(query.Length - 1, 1);
            query.Append(')');
            return query.ToString();
        }

        public string BuildBulkUpdateQuery<T>(IEnumerable<T> entities, GenericRepository<T> repository) where T : IDataObject
        {
            /*
            UPDATE table_name m
            JOIN(
                SELECT 1 as id, 50 as _col1, 30 as _col2
                UNION ALL
                SELECT 2, 5, 40
            ) vals ON m.id = vals.id
            SET col1 = _col1, col2 = _col2 */
            var modelDescription = GetModelDescription<T>();
            var query = new StringBuilder($"UPDATE {repository.TableName} t1 JOIN(SELECT");
            var queryParams = GetQueryParams<T>();
            foreach (var queryParam in queryParams)
            {
                query.Append($"-1000 as _{queryParam},");
            }
            query.Remove(query.Length - 1, 1);
            foreach (var entity in entities)
            {
                query.Append(" UNION ALL SELECT ");
                foreach (var queryParam in queryParams)
                {
                    // Treat null as MySQL NULL instead of default type value
                    var value = modelDescription.GetPropertyValueByName(entity, queryParam);
                    if (value == null)
                    {
                        query.Append("NULL,");
                    }
                    else
                    {
                        if (value is DateTime)
                        {
                            value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (value is bool)
                        {
                            value = ((bool)value) == true ? "1" : "0";
                        }
                        query.Append($"'{value}',");
                    }
                }
                query.Remove(query.Length - 1, 1);
            }
            query.Append($") t2 ON t1.{nameof(IDataObject.ID)} = t2._{nameof(IDataObject.ID)} SET ");
            foreach (var queryParam in queryParams)
            {
                query.Append($"{queryParam} = _{queryParam},");
            }
            query.Remove(query.Length - 1, 1);
            return query.ToString();
        }

        private IReadOnlyList<string> GetQueryParams<T>() where T : IDataObject
        {
            var modeDescription = GetModelDescription<T>();
            return modeDescription.Properties;
        }

        private ModelDescription<T> GetModelDescription<T>() where T : IDataObject
        {
            var classType = typeof(T);
            var className = classType.Name;
            if (!_modelDescriptionCache.TryGetValue(className, out ModelDescription findedModelDescription))
            {
                var modelDescription = BuildAndCacheModelDescription<T>(classType);
                _modelDescriptionCache.TryAdd(className, modelDescription);
                return modelDescription;
            }
            else
            {
                return (ModelDescription<T>)findedModelDescription;
            }
        }

        private static ModelDescription<T> BuildAndCacheModelDescription<T>(Type classType)
        {
            var modelDescription = new ModelDescription<T>();
            var idPropertyName = nameof(IDataObject.ID);
            var modelProperties = GenerateListOfProperties(classType.GetProperties());
            // Fix for dapper split
            modelProperties.Remove(idPropertyName);
            modelProperties.Insert(0, idPropertyName);
            modelDescription.Properties = modelProperties.AsReadOnly();
            foreach (var propertyName in modelProperties)
            {
                modelDescription.BuildGetAccessor(propertyName, classType);
            }
            return modelDescription;
        }

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(NotExistsInDatabaseTable), false)
                    where attributes.Length <= 0
                    select prop.Name).ToList();
        }

        private static string GetJoinTableStringForComplexQuery<T>(string tableNameAlias, ComplexQueryTable<T> table) where T : IDataObject
        {
            return table.JoinType switch
            {
                ComplexQueryTableJoinType.LeftJoin => $" LEFT JOIN {table.TableName} {tableNameAlias} {table.OnCondition}",
                ComplexQueryTableJoinType.RightJoin => $" RIGHT JOIN {table.TableName} {tableNameAlias} {table.OnCondition}",
                ComplexQueryTableJoinType.FullJoin => $" FULL JOIN {table.TableName} {tableNameAlias} {table.OnCondition}",
                ComplexQueryTableJoinType.Join => $" JOIN {table.TableName} {tableNameAlias} {table.OnCondition}",
                _ => "",
            };
        }
    }
}
