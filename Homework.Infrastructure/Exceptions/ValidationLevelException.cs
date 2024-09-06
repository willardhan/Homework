using System;

namespace Homework.Infrastructure.Exceptions
{
    /// <summary>
    /// validation level exception
    /// </summary>
    public class ValidationLevelException : Exception
    {
        public ValidationLevelException()
        { 
        
        }

        public ValidationLevelException(string message) : base(message)
        {

        }
    }
}

