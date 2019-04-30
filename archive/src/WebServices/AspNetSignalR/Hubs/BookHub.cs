using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using AspNetSignalR.Models;

namespace AspNetSignalR.Hubs
{
    public enum UpdateKinds
    {
        Created,
        Updated,
        Deleted,
    }

    public class BookHub : Hub<IBookClient>
    {
        public void BookUpdated(Book book, UpdateKinds kind)
        {
            Clients.All.bookUpdate(book, kind);
        }
    }

    public interface IBookClient
    {
        void bookUpdate(Book book, UpdateKinds kind);
    }
}