using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpcMicroService.Models;
using ProductGrpcMicroService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductGrpcMicroService.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                     .ForMember(dest => dest.CreatedTime,
                                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedTime)));

            CreateMap<ProductModel, Product>()
                    .ForMember(dest => dest.CreatedTime,
                               opt => opt.MapFrom(src => src.CreatedTime.ToDateTime()));

        }
    }
}
