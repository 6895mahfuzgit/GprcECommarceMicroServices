using AutoMapper;
using ShoppingCartGrpcMicroserviceApp.Models;
using ShoppingCartGrpcMicroserviceApp.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpcMicroserviceApp.Mapper
{
    public class ShoppingCartProfile :Profile
    {

        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartModel>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemModel>().ReverseMap();
        }
    }
}
