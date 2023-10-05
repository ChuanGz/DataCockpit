namespace DataCockpit
{
    public interface IRepository<TKey, TModel> where TModel : ModelBase<TKey>
    {
        Task<TModel> AddAsync(TModel model);

        Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models);

        Task DeleteAsync(TKey id);

        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel> GetAsync(TKey id);

        Task<TModel> UpdateAsync(TKey id, TModel model);

        Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models);
    }
}