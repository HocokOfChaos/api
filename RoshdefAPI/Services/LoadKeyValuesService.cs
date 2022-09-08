using RoshdefAPI.Shared.Services.Core;

namespace RoshdefAPI.Services
{
    public class LoadKeyValuesService : IHostedService
    {
        private readonly ILogger<LoadKeyValuesService> _logger;
        private readonly IShopItemsService _shopItemsService;
        private readonly IQuestsService _questsService;

        public LoadKeyValuesService(ILogger<LoadKeyValuesService> logger, IShopItemsService shopItemsService, IQuestsService questsService)
        {
            _logger = logger;
            _shopItemsService = shopItemsService;
            _questsService = questsService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Loading {nameof(IQuestsService)} key values...");
            _questsService.LoadKeyValues();
            _logger.LogInformation($"Loading {nameof(IShopItemsService)} key values...");
            _shopItemsService.LoadKeyValues();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
