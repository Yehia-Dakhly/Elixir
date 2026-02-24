using Serilog.Context;

namespace Blood_Donation.MiddleWares
{
    public class CorrelationalIdMiddleWare(RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var CorrelationId = Guid.NewGuid().ToString("N")[..8].ToUpper();

            context.Items["CorrelationId"] = CorrelationId;

            using (LogContext.PushProperty("CorrelationId", CorrelationId))
            {
                await _next.Invoke(context);
            }
        }
    }
}
