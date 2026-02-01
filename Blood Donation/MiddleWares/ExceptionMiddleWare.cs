using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
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
                    await HttpContext.Response.WriteAsJsonAsync(Response);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something Went Wrong!");
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
