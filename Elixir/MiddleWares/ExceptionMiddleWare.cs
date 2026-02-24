using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using k8s.KubeConfigModels;
using Shared.ErrorModels;
using System.Text.Json;

namespace Blood_Donation.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> logger;

        public ExceptionMiddleWare(RequestDelegate Next, ILogger<ExceptionMiddleWare> _logger)
        {
            _next = Next;
            logger = _logger;
        }
        // Internal Errors
        // NotFound Objects 404
        // Not Found Endpoint
        // Validation Errors
        public async Task InvokeAsync(HttpContext HttpContext)
        {
            try
            {
                await _next.Invoke(HttpContext);

                if (HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var Response = new ErrorToReturn()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = $"Endpoint {HttpContext.Request.Path} is Not Found!"
                    };
                    logger.LogWarning("Endpoint {RequestPath} is Not Found!", HttpContext.Request.Path);
                    await HttpContext.Response.WriteAsJsonAsync(Response);
                }

            }
            catch (Exception ex)
            {
                // Response Object
                var Response = new ErrorToReturn()
                {
                    //StatusCode = HttpContext.Response.StatusCode,
                    ErrorMessage = ex.Message
                };
                // Set Status Code For Response
                HttpContext.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    ForbiddenException => StatusCodes.Status403Forbidden,
                    BadRequestException badRequestException => GetBadRequestErrors(badRequestException, Response),
                    _ => StatusCodes.Status500InternalServerError
                };

                if (HttpContext.Response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(ex, "A critical Server Error Occurred While Processing Request To {RequestPath}", HttpContext.Request.Path);
                    // Security
                    Response.ErrorMessage = "An unexpected internal server error occurred. Please try again later.";
                }
                else
                {
                    logger.LogWarning(ex, "A Client Error ({StatusCode}) Occurred While Processing Request To {RequestPath}", HttpContext.Response.StatusCode, HttpContext.Request.Path);
                    Response.ErrorMessage = ex.Message;
                }
                Response.StatusCode = HttpContext.Response.StatusCode;
                // Return Object As Json && Set Content Type For Response As Json
                await HttpContext.Response.WriteAsJsonAsync(Response);
            }

        }

        private static int GetBadRequestErrors(BadRequestException badRequestException, ErrorToReturn response)
        {
            response.Errors = badRequestException.Errors;
            return StatusCodes.Status400BadRequest;
        }
    }
}
