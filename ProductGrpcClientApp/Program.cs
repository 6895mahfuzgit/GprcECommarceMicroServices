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

            await GetProductAsync(client);

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

            //Get All Product using C# 9 
            Console.WriteLine("Get All Product List ");
            await GetAllProductAsync(client);

            Console.ReadLine();
        }

        private static async Task GetAllProductAsync(ProductProtoService.ProductProtoServiceClient client)
        {
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
