//using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DataCockpit.EfCore
{
    public class EfDbContext : DbContext
    {
        public EfDbContext(string connString) : base(connString)
        {

        }

        public DbSet<AssetData> Assets { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                this.LogValidationErrors(e);
                throw;
            }
        }

        private void LogValidationErrors(DbEntityValidationException e)
        {
            throw new NotImplementedException();
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                this.LogValidationErrors(e);
                throw;
            }
        }
    }
}