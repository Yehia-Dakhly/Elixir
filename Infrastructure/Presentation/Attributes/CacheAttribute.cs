using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction.Abstractions;
using System.Text;

namespace Blood_Donation.Attributes
{
    internal class CacheAttribute(int DurationInDays = 90) : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheKey = CreateCacheKey(context.HttpContext.Request);

            ICacheService _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheValue = await _cacheService.GetAsync(CacheKey);

            if (CacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = CacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };
                return;
            }

            var ExeContext = await next.Invoke();
            if (ExeContext.Result is OkObjectResult result)
            {
                await _cacheService.SetAsync(CacheKey, result.Value, TimeSpan.FromDays(DurationInDays));
            }
        }

        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder Key = new StringBuilder();
            //Key.Append(request.Path + '?');
            var Controller = request.RouteValues["controller"]?.ToString() ?? "UnkownController";
            var Action = request.RouteValues["action"]?.ToString() ?? "UnkownAction";
            Key.Append($"{Controller}:{Action}:");
            foreach (var item in request.Query.OrderBy(I => I.Key))
            {
                Key.Append($"{item.Key}={item.Value}&");
            }

            return Key.ToString();
        }
    }
}
