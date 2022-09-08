using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoshdefAPI.Admin.Models.DTO;
using RoshdefAPI.Admin.Models.Request;
using RoshdefAPI.Controllers.Core;
using RoshdefAPI.Shared.Services.Core;
using System.Net.Mime;

namespace RoshdefAPI.Admin.Controllers
{
    [Authorize]
    public class ShopItemsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IShopItemsService _shopItemsService;

        public ShopItemsController(
            ILogger<ShopItemsController> logger,
            IMapper mapper,
            IShopItemsService shopItemsService
            ) : base(logger)
        {
            _mapper = mapper;
            _shopItemsService = shopItemsService;
        }

        [Produces(MediaTypeNames.Application.Json), HttpPost, Authorize]
        public ShopItemsGetAllResponse GetAll()
        {
            var response = new ShopItemsGetAllResponse
            {
                Success = true,
                Data = _mapper.Map<List<ShopItemDescriptionDTO>>(_shopItemsService.GetAllItems())
            };
            return response;
        }
    }
}
