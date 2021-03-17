using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpcMicroService.Protos;
using System;
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
            Console.ReadLine();
        }

        private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {

            var addProductResponse = await client.AddProductAsync(new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Test_1",
                    Description = "This is a test_1 product.",
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
