using Microsoft.AspNetCore.Http;

namespace Shared.ErrorModels
{
    public class ValidationErrorToReturn
    {
        public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
        public string Message { get; set; } = "Validation Failed";
        public IEnumerable<ValidationError> Errors { get; set; } = [];
    }
}
