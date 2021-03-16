using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductGrpcMicroService.Data;
using ProductGrpcMicroService.Models;
using ProductGrpcMicroService.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductGrpcMicroService.Services
{
    public class ProductService : ProductProtoService.ProductProtoServiceBase
    {
        private readonly ProductContext _productContext;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductContext productContext, ILogger<ProductService> logger)
        {
            _productContext = productContext;
            _logger = logger;
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {

            var product = await _productContext.Products.FindAsync(request.ProductId);
            if (product == null) { }
            //return base.GetProduct(request, context);
            var productModel = new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            };

            return productModel;
        }

        public override async Task GetAllProducts(GetAllProductRequest request,
                                                IServerStreamWriter<ProductModel> responseStream,
                                                ServerCallContext context)
        {
            var products = await _productContext.Products.ToListAsync();

            foreach (var product in products)
            {
                var productModel = new ProductModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
                };

                await responseStream.WriteAsync(productModel);
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            var product = new Product
            {
                ProductId = request.Product.ProductId,
                Name = request.Product.Name,
                Description = request.Product.Description,
                Price = request.Product.Price,
                Status = Enums.ProductStatus.INSTOCK,
                CreatedTime = request.Product.CreatedTime.ToDateTime()
            };

            _productContext.Products.Add(product);
            await _productContext.SaveChangesAsync();

            var productModel = new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            };
            return productModel;
        }



    }
}
