using SharedLib;
using SharedLib.Models;

namespace WebUi.Services
{
    public class ItemsService
    {
        private readonly ILogger<ItemsService> _logger;
        private readonly IDbActions _dbActions;
        public ItemsService(ILogger<ItemsService> logger, IDbActions dbActions) 
        {
            _logger = logger;
            _dbActions = dbActions;
        }

        public async IAsyncEnumerable<TemplateUpdate> GetAllItemsAsync()
        {
            await foreach (var item in await _dbActions.GetAll())
            {
                yield return item;
            }
        }
        public async Task UpdateItemAsync(TemplateUpdate templateUpdate)
        {
            await _dbActions.Update(templateUpdate);
        }
        public async Task AddItemAsync(TemplateUpdate templateUpdate)
        {
            await _dbActions.Add(templateUpdate);
        }
    }
}
