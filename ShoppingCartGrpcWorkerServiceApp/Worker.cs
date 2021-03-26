using Grpc.Core;
using Grpc.Net.Client;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductGrpcMicroService.Protos;
using ShoppingCartGrpcMicroserviceApp.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCartGrpcWorkerServiceApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread.Sleep(3000);
            while (!stoppingToken.IsCancellationRequested)
            {

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using var scChannel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ShoppingCartServerURL"));
                var scClient = new ShoppingCartProtoService.ShoppingCartProtoServiceClient(scChannel);
                //Get token from identity Server
                var token = await GetTokenFromIdentityServer();

                var scModel = await GetOrCreateShoppingModel(scClient);


                using var scClientStraem = scClient.AddItemIntoShopppingCart();
                // get Products
                using var productCannel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ProductServerURL"));
                var productClient = new ProductProtoService.ProductProtoServiceClient(productCannel);

                _logger.LogInformation("*****GetAllProducts Started****");
                using var clientData = productClient.GetAllProducts(new GetAllProductRequest());
                await foreach (var product in clientData.ResponseStream.ReadAllAsync())
                {
                    _logger.LogInformation("--Each Product--" + product);

                    var newSCItem = new AddItemIntoShopppingCartRequest
                    {
                        Username = _configuration.GetValue<string>("WorkerService:UserName"),
                        DiscountCode = "D100",
                        NewCartItem = new ShoppingCartItemModel
                        {
                            ProductId = product.ProductId,
                            Color = "GREEN",
                            Price = product.Price,
                            Productname = product.Name,
                            Quantity = 1
                        }
                    };

                    await scClientStraem.RequestStream.WriteAsync(newSCItem);

                    _logger.LogInformation("Written Client Stream" + newSCItem.ToString());

                }

                await scClientStraem.RequestStream.CompleteAsync();

                var addItemClientStreamResponse = await scClientStraem;

                _logger.LogInformation("Add Client Stream Retsponse" + addItemClientStreamResponse.ToString());
                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }

        private async Task<string> GetTokenFromIdentityServer()
        {
            var client = new HttpClient();
            var discover = await client.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("WorkerService:IdentityServer"));

            if (discover.IsError)
            {
                Console.WriteLine(discover.Error);
                return string.Empty;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discover.TokenEndpoint,
                ClientId = "ShoppingCartClient",
                ClientSecret = "secret",
                Scope = "ShoppingCartAPI"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(discover.Error);
                return string.Empty;
            }

            return tokenResponse.AccessToken;
        }

        private async Task<ShoppingCartModel> GetOrCreateShoppingModel(ShoppingCartProtoService.ShoppingCartProtoServiceClient scClient)
        {
            ShoppingCartModel shoppingCartModel;
            try
            {
                _logger.LogInformation("GetShoppingCartAsync Started.......");

                shoppingCartModel = await scClient.GetShoppigCartAsync(new GetShoppigCartRequest
                {
                    Username = _configuration.GetValue<string>("WorkerService:UserName")
                });

                _logger.LogInformation($"Response From GetShoppigCartAsync {shoppingCartModel.ToString()}");

            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogInformation("CreateShoppigCartAsync Started.......");
                    shoppingCartModel = await scClient.CreateShoppigCartAsync(new ShoppingCartModel
                    {
                        Username = _configuration.GetValue<string>("WorkerService:UserName")
                    });

                    _logger.LogInformation("CreateShoppigCartAsync Response " + shoppingCartModel.ToString());
                }
                else
                {
                    throw ex;
                }
            }

            return shoppingCartModel;
        }


    }
}
