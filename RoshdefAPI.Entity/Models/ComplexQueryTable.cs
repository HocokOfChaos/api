using RoshdefAPI.Entity.Repositories.Core;

namespace RoshdefAPI.Entity.Models
{
    public enum ComplexQueryTableJoinType
    {
        NotUsedInJoin,
        Join,
        LeftJoin,
        RightJoin,
        FullJoin
    }

    public class ComplexQueryTable<T>
    {
        public readonly ComplexQueryTableJoinType JoinType;
        public readonly string ClassName;

        public readonly string OnCondition;

        public readonly string TableName;

        public ComplexQueryTable(ComplexQueryTableJoinType joinType, GenericRepository repository, string? onCondition = null)
        {
            ClassName = typeof(T).Name;
            JoinType = joinType;
            TableName = repository.TableName;
            OnCondition = onCondition ?? string.Empty;
        }
    }
}
