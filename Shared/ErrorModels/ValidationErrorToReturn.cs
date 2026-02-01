using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ValidationErrorToReturn
    {
        public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
        public string Message { get; set; } = "Validation Failed";
        public IEnumerable<ValidationError> ValidationErrors { get; set; } = [];
    }
}
