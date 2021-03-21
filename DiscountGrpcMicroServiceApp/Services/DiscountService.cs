using AutoMapper;
using DiscountGrpcMicroServiceApp.Data;
using DiscountGrpcMicroServiceApp.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountGrpcMicroServiceApp.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {

        private readonly ILogger<DiscountService> _logger;
        private readonly DiscountContext _discountContext;
        private readonly IMapper _mapper;

        public DiscountService(ILogger<DiscountService> logger, DiscountContext discountContext, IMapper mapper)
        {
            _logger = logger;
            _discountContext = discountContext;
            _mapper = mapper;
        }

        public override async Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var discountFromDb = await _discountContext.Discounts.FirstOrDefaultAsync(x => x.Code == request.DiscountCode);

            if (discountFromDb == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,"Invalid Code Request"));
            }

            var discountModel = _mapper.Map<DiscountModel>(discountFromDb);
            return await Task.FromResult(discountModel);

        }
    }



}
