using RoshdefAPI.Admin.Services.Core;
using RoshdefAPI.Shared.Services.Core;

namespace RoshdefAPI.Admin.Services
{
    public class PreloadSingletonsService : IHostedService
    {
        private readonly ILogger<PreloadSingletonsService> _logger;
        private readonly IShopItemsService _shopItemsService;
        private readonly IDOTALocalizationService _dotaLocalizationService;
        private readonly IJsonStringLocalizer _jsonStringLocalizer;

        public PreloadSingletonsService(ILogger<PreloadSingletonsService> logger, IShopItemsService shopItemsService, IDOTALocalizationService localizationService, IJsonStringLocalizer jsonStringLocalizer)
        {
            _logger = logger;
            _shopItemsService = shopItemsService;
            _dotaLocalizationService = localizationService;
            _jsonStringLocalizer = jsonStringLocalizer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Loading {nameof(IDOTALocalizationService)}...");
            _dotaLocalizationService.LoadKeyValues();
            _logger.LogInformation($"Loading {nameof(IShopItemsService)}...");
            _shopItemsService.LoadKeyValues();
            _logger.LogInformation($"Loading {nameof(IJsonStringLocalizer)}...");
            _jsonStringLocalizer.LoadLocalization();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
