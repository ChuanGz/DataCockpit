namespace DataCockpit
{
    public abstract class CrudBase<TKey, TModel, TRepository> : ICrud<TKey, TModel>
        where TModel : ModelBase<TKey>
        where TRepository : IRepository<TKey, TModel>
    {
        protected readonly TRepository repository;

        public CrudBase(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual async Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models) => await repository.AddAsync(models);

        public virtual async Task<TModel> AddAsync(TModel model) => await repository.AddAsync(model);

        public virtual async Task DeleteAsync(TKey key) => await repository.DeleteAsync(key);

        public virtual async Task<TModel> GetAsync(TKey key) => await repository.GetAsync(key);

        public virtual async Task<IEnumerable<TModel>> ListAsync() => await repository.GetAllAsync();

        public virtual async Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models) => await repository.UpdateAsync(models);

        public virtual async Task<TModel> UpdateAsync(TKey key, TModel model) => await repository.UpdateAsync(key, model);
    }
}