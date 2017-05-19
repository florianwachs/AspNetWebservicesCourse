using System;

namespace AspNetCore.BooksServer.Infrastructure
{
    public class DefaultTimeService : ITimeService
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime Today
        {
            get
            {
                return DateTime.Today;
            }
        }
    }
}
