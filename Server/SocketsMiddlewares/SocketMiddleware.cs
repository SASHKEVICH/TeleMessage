
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Server.Services;

namespace Server.SocketsMiddlewares
{
    public abstract class SocketMiddleware
    {
        protected RequestDelegate _next;
        private SocketService Handler { get; set; }

        protected SocketMiddleware(RequestDelegate next, SocketService handler)
        {
            _next = next;
            Handler = handler;
        }

        public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);
    }
}