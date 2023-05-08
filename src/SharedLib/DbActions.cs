using Microsoft.EntityFrameworkCore;
using SharedLib.Models;

namespace SharedLib
{
    public class DbActions : IDbActions
    {
        private readonly DatabaseConnection _db;
        public DbActions(DatabaseConnection db) 
        {
            _db = db;
        }
        public async Task<IAsyncEnumerable<TemplateUpdate>> GetAll()
        {
            try
            {
                return _db.TemplateUpdate.AsAsyncEnumerable();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<TemplateUpdate> GetByFriendlyName(string friendlyName)
        {
            try
            {
                return await _db.TemplateUpdate.Where(t => t.FriendlyName == friendlyName).FirstAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Add(TemplateUpdate templateUpdate)
        {
            try
            {
                _db.TemplateUpdate.Add(templateUpdate);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task Update(TemplateUpdate templateUpdate)
        {
            try
            {
                _db.TemplateUpdate.Update(templateUpdate);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public interface IDbActions
    {
        Task<IAsyncEnumerable<TemplateUpdate>> GetAll();
        public Task<TemplateUpdate> GetByFriendlyName(string friendlyName);
        public Task Add(TemplateUpdate templateUpdate);
        public Task Update(TemplateUpdate templateUpdate);
    }
}
