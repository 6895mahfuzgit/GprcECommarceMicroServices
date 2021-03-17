using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductContext productContext, IMapper mapper, ILogger<ProductService> logger)
        {
            _productContext = productContext;
            _mapper = mapper;
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
            //var productModel = new ProductModel
            //{
            //    ProductId = product.ProductId,
            //    Name = product.Name,
            //    Description = product.Description,
            //    Price = product.Price,
            //    Status = ProductStatus.Instock,
            //    CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            //};

            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }

        public override async Task GetAllProducts(GetAllProductRequest request,
                                                IServerStreamWriter<ProductModel> responseStream,
                                                ServerCallContext context)
        {
            var products = await _productContext.Products.ToListAsync();

            foreach (var product in products)
            {
                //var productModel = new ProductModel
                //{
                //    ProductId = product.ProductId,
                //    Name = product.Name,
                //    Description = product.Description,
                //    Price = product.Price,
                //    Status = ProductStatus.Instock,
                //    CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
                //};

                var productModel = _mapper.Map<ProductModel>(product);
                await responseStream.WriteAsync(productModel);
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            //var product = new Product
            //{
            //    ProductId = request.Product.ProductId,
            //    Name = request.Product.Name,
            //    Description = request.Product.Description,
            //    Price = request.Product.Price,
            //    Status = Enums.ProductStatus.INSTOCK,
            //    CreatedTime = request.Product.CreatedTime.ToDateTime()
            //};
            var product = _mapper.Map<Product>(request.Product);
            _productContext.Products.Add(product);
            await _productContext.SaveChangesAsync();

            //var productModel = new ProductModel
            //{
            //    ProductId = product.ProductId,
            //    Name = product.Name,
            //    Description = product.Description,
            //    Price = product.Price,
            //    Status = ProductStatus.Instock,
            //    CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            //};
            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }


        public override async Task<ProductModel> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var product = _mapper.Map<Product>(request.Product);

            bool isExist = await _productContext.Products.AnyAsync(x => x.ProductId == product.ProductId);
            if (!isExist)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Requested product doesn't exists!!!"));
            }

            _productContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _productContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            var productModel = _mapper.Map<ProductModel>(product);
            return productModel;
        }


        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            var product = await _productContext.Products.FindAsync(request.ProductId);

            if (product == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Requested product doesn't exists!!!"));
            }

            _productContext.Products.Remove(product);

            var deleteCount = await _productContext.SaveChangesAsync();

            var response = new DeleteProductResponse
            {
                Success = deleteCount > 0
            };

            return response;
        }

        public override async Task<InsertBulkProductResponse> InsertBulkProduct(IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var product = _mapper.Map<Product>(requestStream.Current);
                _productContext.Products.Add(product);
            }

            var countProduct = await _productContext.SaveChangesAsync();

            var response = new InsertBulkProductResponse
            {
                InsertCount = countProduct,
                Success = countProduct > 0
            };

            return response;
        }

    }
}
