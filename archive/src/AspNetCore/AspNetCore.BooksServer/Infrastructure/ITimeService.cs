using System;

namespace AspNetCore.BooksServer.Infrastructure
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}
