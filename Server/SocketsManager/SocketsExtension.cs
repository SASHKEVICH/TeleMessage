using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Server.Services;
using Server.SocketsMiddlewares;

namespace Server.SocketsManager
{
    public static class SocketsExtension
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<ConnectionManager>();
            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(SocketService))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }

        public static IApplicationBuilder MapSockets(this IApplicationBuilder app, PathString path,
            SocketService socket)
        {
            return app.Map(path, x => x.UseMiddleware<SocketMiddleware>(socket));
        }
    }
}