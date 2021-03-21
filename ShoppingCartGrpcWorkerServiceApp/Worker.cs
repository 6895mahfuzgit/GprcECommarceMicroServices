using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCartGrpcMicroserviceApp.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var scModel = await GetOrCreateShoppingModel(scClient);

                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
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
