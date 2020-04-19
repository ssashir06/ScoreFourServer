using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ScoreFourServer.Domain.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ActionFilters
{
    public sealed class ClientTokenActionFilterAttribute : ActionFilterAttribute
    {
        static Lazy<Regex> lazyRegex  = new Lazy<Regex>(() => new Regex(@"Bearer ([a-zA-Z0-9-._~+/]+[=]*)"));

        public ClientTokenActionFilterAttribute(IClientTokenAdapter clientTokenAdapter)
        {
            ClientTokenAdapter = clientTokenAdapter;
        }

        public IClientTokenAdapter ClientTokenAdapter { get; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var value))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var match = lazyRegex.Value.Match(value.ToString());
            if (!match.Success)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            Guid accessKey;
            try
            {
                accessKey = new Guid(Convert.FromBase64String(match.Groups[1].Value));
            }
            catch
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var clientToken = await ClientTokenAdapter.GetAsync(accessKey, context.HttpContext.RequestAborted);
            if (clientToken ==null || clientToken.IsExpired)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.HttpContext.Items["ClientToken"] = clientToken;

            await next();
        }
    }
}
