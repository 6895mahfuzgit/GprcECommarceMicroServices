using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpcMicroService.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductGrpcClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ProductProtoService.ProductProtoServiceClient(channel);



            // Get All Product
            Console.WriteLine("*****************************");
            //Console.WriteLine("Get All Product List");
            //using (var clientData = client.GetAllProducts(new GetAllProductRequest()))
            //{
            //    while (await clientData.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
            //    {
            //        var currentProduct = clientData.ResponseStream.Current;
            //        Console.WriteLine(currentProduct.ToString());
            //    }

            //}


            await GetProductAsync(client);
            Console.WriteLine("Get All Product List ");
            await GetAllProductAsync(client);
            await AddProductAsync(client);
            Console.WriteLine("");
            Console.WriteLine("");

            await UpdateProductAsync(client);
            Console.WriteLine("Get All Product List ");
            await GetAllProductAsync(client);
            Console.WriteLine("");
            Console.WriteLine("");

            await DeleteProductAsync(client);
            Console.WriteLine("Get All Product List ");
            await GetAllProductAsync(client);
            Console.WriteLine("");
            Console.WriteLine("");

            await InsertBulkProductAsync(client);
            Console.WriteLine("Get All Product List ");
            await GetAllProductAsync(client);

            Console.ReadLine();
        }

        private static async Task InsertBulkProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {

            using var clientBulk = client.InsertBulkProduct();

            var listProduct = new List<ProductModel> {
            new ProductModel{Name="Product_1",Description="Description_1",Price=100,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_2",Description="Description_2",Price=200,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_3",Description="Description_3",Price=300,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_4",Description="Description_4",Price=400,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },new ProductModel{Name="Product_",Description="Description_",Price=100,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_5",Description="Description_5",Price=500,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_6",Description="Description_6",Price=600,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_7",Description="Description_7",Price=700,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_8",Description="Description_8",Price=800,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },new ProductModel{Name="Product_",Description="Description_",Price=100,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            new ProductModel{Name="Product_9",Description="Description_9",Price=900,Status=ProductStatus.Instock,CreatedTime=Timestamp.FromDateTime(DateTime.UtcNow) },
            };

            foreach (var product in listProduct)
            {
                await clientBulk.RequestStream.WriteAsync(product);
            }

            await clientBulk.RequestStream.CompleteAsync();

            var response = await clientBulk;

            Console.WriteLine("Total Inserted Product: " + response.InsertCount + " Status: " + response.Success);

        }

        private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            var replay = await client.DeleteProductAsync(new DeleteProductRequest
            {
                ProductId = 4
            });

            Console.WriteLine("Deleted Product Status : " + replay.ToString());
        }

        private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            var replay = await client.UpdateProductAsync(new UpdateProductRequest
            {
                Product = new ProductModel
                {
                    ProductId = 4,
                    Name = "Test_2_1",
                    Description = "This is a test_2_1 product.",
                    Price = 100,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

            Console.WriteLine("After Update Product Obj:-" + replay.ToString());
        }

        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {

            var addProductResponse = await client.AddProductAsync(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Test_2",
                    Description = "This is a test_2 product.",
                    Price = 100,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            }); ;

            Console.WriteLine("Added Product:-" + addProductResponse.ToString());

        }

        private static async Task GetAllProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            //Get All Product using C# 9 
            using var clientData = client.GetAllProducts(new GetAllProductRequest());
            await foreach (var product in clientData.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(product);
            }
        }

        private static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
            var reply = await client.GetProductAsync(new GetProductRequest
            {
                ProductId = 1
            });

            Console.WriteLine(reply.ToString());
        }
    }
}
