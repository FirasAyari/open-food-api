using System;

namespace open_food_api.Infrastructure.Services.Exceptions
{
    public class InvalidProductStatusException : Exception
    {
        public InvalidProductStatusException() : base("Invalid product status.")
        {
        }

        public InvalidProductStatusException(string message) : base(message)
        {
        }

        public InvalidProductStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
