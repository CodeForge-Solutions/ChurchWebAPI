using Microsoft.AspNetCore.Mvc.Controllers;

namespace churchWebAPI.Services.Middleware
{
    public class ControllerNameMiddleware
    {
        private readonly RequestDelegate _next;

        public ControllerNameMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (actionDescriptor != null)
                {
                    var controllerName = actionDescriptor.ControllerName;
                    var methodName = actionDescriptor.ActionName;
                    context.Items["ControllerName"] = controllerName;
                    context.Items["MethodName"] = methodName;
                }
            }

            await _next(context);
        }
    }
}
