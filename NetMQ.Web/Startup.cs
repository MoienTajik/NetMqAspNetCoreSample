using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetMQ.Sockets;

namespace NetMQ.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<NetMqBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }

    public class NetMqBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var server = new ResponseSocket("tcp://*:8050");

            while (!stoppingToken.IsCancellationRequested)
            {
                string serverMessage = server.ReceiveFrameString();
                Console.WriteLine($"Client Sent: {serverMessage}");

                Console.WriteLine("Sending reply to the client ...");

                const string msg1 = "Hi", msg2 = "How are you ?";

                var response = new NetMQMessage();
                response.Append(msg1);
                response.Append(msg2);

                server.SendMultipartMessage(response);
                Console.WriteLine($"Response has been sent => 1: {msg1}, 2: {msg2}");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}