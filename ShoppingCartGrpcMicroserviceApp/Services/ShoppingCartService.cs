using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpcMicroserviceApp.Data;
using ShoppingCartGrpcMicroserviceApp.Models;
using ShoppingCartGrpcMicroserviceApp.Protos;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpcMicroserviceApp.Services
{
     [Authorize]
    public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
    {
        private readonly ILogger<ShoppingCartService> _logger;
        private readonly ShoppingCartContext _shoppingCartContext;
        private readonly IMapper _mapper;
        private readonly DiscountService _discountService;

        public ShoppingCartService(ILogger<ShoppingCartService> logger, ShoppingCartContext shoppingCartContext, IMapper mapper, DiscountService discountService)
        {
            _logger = logger;
            _shoppingCartContext = shoppingCartContext;
            _mapper = mapper;
            _discountService = discountService;
        }

        public override async Task<ShoppingCartModel> GetShoppigCart(GetShoppigCartRequest request, ServerCallContext context)
        {
            var shoppingCart = await _shoppingCartContext.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.Username);
            if (shoppingCart == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid User Cart"));
            }

            var shoppingCartModel = _mapper.Map<ShoppingCartModel>(shoppingCart);

            
            return shoppingCartModel;
        }

        public override async Task<ShoppingCartModel> CreateShoppigCart(ShoppingCartModel request, ServerCallContext context)
        {

            var shoppingCart = _mapper.Map<ShoppingCart>(request);
            var isExists = await _shoppingCartContext.ShoppingCarts.AnyAsync(x => x.UserName == shoppingCart.UserName);
            if (isExists)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid Request."));
            }

            _shoppingCartContext.ShoppingCarts.Add(shoppingCart);
            await _shoppingCartContext.SaveChangesAsync();

            var shoppingCartModel = _mapper.Map<ShoppingCartModel>(request);
            return shoppingCartModel;
        }

        [AllowAnonymous]
        public override async Task<RemoveItemFromShoppingCartResponse> RemoveItemFromShoppingCart(RemoveItemFromShoppingCartRequest request, ServerCallContext context)
        {
            var shoppingCart = await _shoppingCartContext.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == request.Username);
            if (shoppingCart == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid Request."));
            }

            var removeCartItem = shoppingCart.Items.FirstOrDefault(x => x.ProductId == request.RemoveCartItem.ProductId);

            if (removeCartItem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Invalid Product Item."));
            }

            _shoppingCartContext.Remove(removeCartItem);

            var removeCount = await _shoppingCartContext.SaveChangesAsync();

            var response = new RemoveItemFromShoppingCartResponse
            {
                Success = removeCount > 0
            };

            return response;
        }

        [AllowAnonymous]
        public override async Task<AddItemIntoShopppingCartResponse> AddItemIntoShopppingCart(IAsyncStreamReader<AddItemIntoShopppingCartRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {

                var shoppingCart = await _shoppingCartContext.ShoppingCarts.FirstOrDefaultAsync(x => x.UserName == requestStream.Current.Username);
                if (shoppingCart == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Invalid Request."));
                }


                var addedNewCartItem = _mapper.Map<ShoppingCartItem>(requestStream.Current.NewCartItem);
                var cartItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == addedNewCartItem.ProductId);
                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {

                    var disCount = await _discountService.GetDiscount(requestStream.Current.DiscountCode);
                    addedNewCartItem.Price -= disCount.Amount;
                    shoppingCart.Items.Add(addedNewCartItem);
                }


            }

            var insertCount = await _shoppingCartContext.SaveChangesAsync();
            var reply = new AddItemIntoShopppingCartResponse
            {
                Success = insertCount > 0,
                InsertCount = insertCount
            };

            return reply;

        }

    }
}
