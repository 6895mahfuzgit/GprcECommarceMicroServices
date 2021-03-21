using AutoMapper;
using DiscountGrpcMicroServiceApp.Model;
using DiscountGrpcMicroServiceApp.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountGrpcMicroServiceApp.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Discount, DiscountModel>().ReverseMap();
        }
    }
}
