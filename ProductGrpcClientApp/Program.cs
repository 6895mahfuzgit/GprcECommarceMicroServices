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

            var reply = await client.GetProductAsync(new GetProductRequest
            {
                ProductId=1
            });

            Console.WriteLine(reply.ToString());
            Console.ReadLine();
        }
    }
}
