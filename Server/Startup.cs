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
        #region Properties

        private IConfiguration Configuration { get; }

        #endregion

        #region Constructor

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
            services.AddDbContext<ChatContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("ChatContext"));
            });
            services.AddScoped<IRepository, Repository>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            InitializeDatabase(app);
            
            app.UseWebSockets();
            app.MapSockets("/connecting", serviceProvider.GetService<ConnectionHanlder>());
            app.MapSockets("/message", serviceProvider.GetService<MessageHandler>());
            app.UseStaticFiles();
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<ChatContext>().Database.Migrate();
        }

        #endregion
    }
}