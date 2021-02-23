using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRChat.Hubs;
using SignalRChat.Services;

namespace signalRChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            //services.AddScoped<RabbitMQService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                 endpoints.MapHub<ChatHub>("/chathub");
            });

            
            RegisterSignalRWithRabbitMQ(app.ApplicationServices.GetService<IRabbitMQService>());
            // Run 'RegisterSignalRWithRabbitMQ' when the application has started.
            // lifetime.ApplicationStarted.Register(() => RegisterSignalRWithRabbitMQ(app.ApplicationServices));
            
       
        }
    
        public void RegisterSignalRWithRabbitMQ(IRabbitMQService service)
        {
            // var factory = new ConnectionFactory() { HostName = "localhost"  };
            // using (var connection = factory.CreateConnection())
            // {
            //     using (var channel = connection.CreateModel())
            //     {
            //         channel.QueueDeclare(queue: "TestQueue",
            //         durable: false,
            //         autoDelete: false,
            //         exclusive: false,
            //         arguments: null
            //         );

            //         var consumer = new EventingBasicConsumer(channel);
            //         consumer.Received += (model, ea) =>
            //         {
            //               var body = ea.Body.ToArray();
            //               var message = Encoding.UTF8.GetString(body);  
            //             // Console.WriteLine(" [x] Received {0}", message);
            //         };
                    


            //         channel.BasicConsume(queue: "TestQueue",
            //         autoAck : true,
            //         consumer: consumer);
                    
            //         //Console.WriteLine(" Press any key to exit ");
            //         //Console.ReadKey();

            //     }
            // }
            // Connect to RabbitMQ
            var rabbitMQService = service;
            rabbitMQService.Connect();
        }
        
    }
}
