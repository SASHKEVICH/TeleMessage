using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.ApplicationContext;
using Server.DataBase;
using Server.Handlers;
using Server.SocketsManager;

namespace Server
{
    public class Startup
    {
        private IConfiguration _configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
            services.AddDbContext<ChatContext>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("ChatContext"));
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.MapSockets("/message", serviceProvider.GetService<MessageHandler>());
            app.UseStaticFiles();

            var repository = new Repository();
            repository.Initialize(serviceProvider);
        }
    }
}