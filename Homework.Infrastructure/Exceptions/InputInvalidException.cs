using System;

namespace Homework.Infrastructure.Exceptions
{
    /// <summary>
    /// input invalid exception
    /// </summary>
    public class InputInvalidException : Exception
    {
        public InputInvalidException()
        { 
        
        }

        public InputInvalidException(string message) : base(message)
        {

        }
    }
}

