using DiscountGrpcMicroServiceApp.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpcMicroserviceApp.Services
{
    public class DiscountService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoServiceClient;

        public DiscountService(DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient)
        {
            _discountProtoServiceClient = discountProtoServiceClient;
        }


        public async Task<DiscountModel> GetDiscount(string code)
        {
            var request = new GetDiscountRequest
            {
                DiscountCode = code
            };

            return await _discountProtoServiceClient.GetDiscountAsync(request);
        }

    }
