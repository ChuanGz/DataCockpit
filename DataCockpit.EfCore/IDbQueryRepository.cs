using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCockpit.EfCore
{
    public interface IDbQueryRepository<TKey, TData, TModel>
        where TData : EntityBase<TKey>
        where TModel : ModelBase<TKey>
        where TKey : IEquatable<TKey>
    {
        DbRepositoryBase<TKey, TData, TModel> DbBaseRepository { get; }
    }
}
