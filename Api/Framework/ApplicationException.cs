using System;

namespace Api.Framework
{
    public class ApplicationException : Exception
    {
        public string Code { get; set; }

        public ApplicationException()
        {

        }

        public ApplicationException(string message, string code)
            : base(message)
        {
            Code = code;
        }
    }
}