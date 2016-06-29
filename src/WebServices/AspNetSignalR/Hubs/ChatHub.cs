using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetSignalR.Hubs
{
    // Basiert auf http://www.asp.net/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc
    //[HubName("MyChatHub")]
    public class ChatHub : Hub
    {
        private static int currentUsers = 0;

        //[HubMethodName("MySend")]
        public void Send(string name, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message))
            {
                var errorMessage = "Chuck Says: Invalid Message, Roundhouse Kick for YOU!";
                Clients.Caller.invalidMessage(errorMessage);
                return;
            }

            Clients.All.newMessage(name, message);
        }

        public int GetCurrentUserCount()
        {
            return currentUsers;
        }

        public override Task OnConnected()
        {
            var cnt = Interlocked.Increment(ref currentUsers);
            Clients.All.currentUserUpdate(cnt);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var cnt = Interlocked.Decrement(ref currentUsers);
            Clients.All.currentUserUpdate(cnt);
            return base.OnDisconnected(stopCalled);
        }
    }



    public interface IChat
    {
        void invalidMessage(string msg);
        void newMessage(string name, string message);
    }

    public class ChatWithInterfaceHub : Hub<IChat>
    {
        public void Send(string name, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message))
            {
                var errorMessage = "Chuck Says: Invalid Message, Roundhouse Kick for YOU!";
                Clients.Caller.invalidMessage(errorMessage);
                return;
            }

            Clients.All.newMessage(name, message);
        }
    }


}