using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.ActionFilters
{
    public class RequestTraceActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Trace.WriteLine($"Executing: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Trace.WriteLine($"Executed: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} (StatusCode={context.HttpContext.Response.StatusCode})");
        }
    }
}
