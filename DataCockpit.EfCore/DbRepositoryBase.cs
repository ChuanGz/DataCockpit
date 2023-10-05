using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace DataCockpit.EfCore
{
    public abstract class DbRepositoryBase<TKey, TData, TModel>
         : IRepository<TKey, TModel>, IDbQueryRepository<TKey, TData, TModel>
         where TData : EntityBase<TKey>
         where TModel : ModelBase<TKey>
         where TKey : IEquatable<TKey>
    {
        public IMapper ObjectMapper { get; set; }

        protected abstract Expression<Func<TData, object>>[] Includes { get; }

        protected EfDbContext EfDbCtx { get; }

        public DbRepositoryBase<TKey, TData, TModel> DbBaseRepository => this;

        public DbRepositoryBase(EfDbContext db, IMapper mapper)
        {
            this.ObjectMapper = mapper.ThrowIfNull(nameof(mapper));
            this.EfDbCtx = db.ThrowIfNull(nameof(db));
        }

        public virtual async Task<TModel> AddAsync(TModel model)
        {
            model.ThrowIfNull(nameof(model));

            var data = this.ObjectMapper.Map<TData>(model);

            this.EfDbCtx.Set<TData>().Add(data);

            await this.EfDbCtx.SaveChangesAsync();

            if (this.Includes != null)
            {
                data = await this.FindAsync(data.Id);
            }

            return ObjectMapper.Map<TModel>(data);
        }

        public virtual async Task<IEnumerable<TModel>> AddAsync(IEnumerable<TModel> models)
        {
            models.ThrowIfNull(nameof(models));

            if (!models.Any())
            {
                return Enumerable.Empty<TModel>();
            }

            var dataModels = models.Select(m => ObjectMapper.Map<TData>(m)).ToArray();

            var resultingData = this.EfDbCtx.Set<TData>().AddRange(dataModels);

            await this.EfDbCtx.SaveChangesAsync();

            return ObjectMapper.Map<IEnumerable<TModel>>(resultingData);
        }


        public virtual async Task DeleteAsync(TKey id)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new Exception("Attempting to delete a data that does not exist");
            }

            this.EfDbCtx.Set<TData>().Attach(data);
            data.Deleted = true;

            await this.EfDbCtx.SaveChangesAsync();
        }

        public virtual async Task<TModel> GetAsync(TKey id)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                return default(TModel);
            }

            return ObjectMapper.Map<TModel>(data);
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var data = await this.FindBy(t => true).ToListAsync();

            return this.ObjectMapper.Map<IEnumerable<TModel>>(data);
        }

        public virtual async Task<TModel> UpdateAsync(TKey id, TModel model)
        {
            var data = await this.FindAsync(id);

            if (data == null)
            {
                throw new Exception("Attempting to update a data that does not exist");
            }

            this.EfDbCtx.Set<TData>().Attach(data);

            UpdateData(data, model);

            await this.EfDbCtx.SaveChangesAsync();

            if (this.Includes != null)
            {
                data = await this.FindAsync(data.Id);
            }

            return ObjectMapper.Map<TModel>(data);
        }

        public virtual async Task<IEnumerable<TModel>> UpdateAsync(IDictionary<TKey, TModel> models)
        {
            var dataModels = await this.FindBy(d => models.Keys.Contains(d.Id)).ToListAsync();

            if (dataModels.Any(d => d == null))
            {
                throw new Exception("Attempting to update a data that does not exist");
            }

            var data = dataModels.ToDictionary(i => i.Id, i => i);

            foreach (var item in data)
            {
                UpdateData(item.Value, models[item.Key]);
            }

            await this.EfDbCtx.UpdateAsync(data.Select(d => d.Value));

            if (this.Includes != null)
            {
                var ids = data.Select(d => d.Key).ToList();
                var res = await this.FindBy(d => ids.Contains(d.Id)).ToListAsync();
                return this.ObjectMapper.Map<IEnumerable<TModel>>(res);
            }

            return this.ObjectMapper.Map<IEnumerable<TModel>>(data);
        }

        protected async Task<TData> FindAsync(TKey id)
        {
            return await this.FindBy(data => data.Id.Equals(id)).SingleOrDefaultAsync();
        }

        public IQueryable<TData> FindBy(Expression<Func<TData, bool>> predicate)
        {
            var dbQuery = this.EfDbCtx.Set<TData>()
                .AsNoTracking()
                .Where(d => !d.Deleted)
                .Where(predicate);

            if (this.Includes != null)
            {
                dbQuery = this.Includes.Aggregate(dbQuery, (current, include) => current.Include(include));
            }

            return dbQuery;
        }


        protected void UpdateData(TData data, TModel model)
        {
            var clonedModel = this.ObjectMapper.Map<TModel>(data);

            var modelProperties = typeof(TModel).GetProperties();
            foreach (var property in modelProperties)
            {
                var attributes = property.GetCustomAttributes();
                if (attributes.Any(a => a.GetType() == typeof(MutableAttribute)))
                {
                    property.SetValue(clonedModel, property.GetValue(model));
                }
            }

            var intermediaryData = this.ObjectMapper.Map<TData>(clonedModel);

            var propertyMap = this.ObjectMapper.ConfigurationProvider.FindTypeMapFor<TModel, TData>();
            var mappedPropertyNames = propertyMap.GetPropertyMaps().Where(m => m.IsMapped()).Select(m => m.DestinationProperty.Name);

            var dataProperties = typeof(TData).GetProperties();
            dataProperties = dataProperties.Where(p => mappedPropertyNames.Contains(p.Name)).ToArray();

            foreach (var property in dataProperties)
            {
                property.SetValue(data, property.GetValue(intermediaryData));
            }

        }
    }
}