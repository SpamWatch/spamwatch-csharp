using System;

namespace SpamWatch
{
    public class Error : Exception
    {
        public Error(string message) : base(message)
        {
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    public class TooManyRequests : Exception
    {
        public DateTime UntilDate;

        public TooManyRequests(string method, DateTime until) : base(
            $"Too Many Requests for method {method}. Try again in {(int) (until - DateTime.Now.ToLocalTime()).TotalSeconds}")
        {
            UntilDate = until;
        }
    }
}