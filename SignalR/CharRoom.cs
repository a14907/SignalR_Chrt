using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalR
{
    public class CharRoom : Hub
    {
        private static List<UserMsg> allUser = new List<UserMsg>();
        public List<UserMsg> SendAll(string fromName, string msg)
        {
            Clients.All.addMsg(fromName, msg);
            return allUser;
        }
        public List<UserMsg> Send(string toId, string fromName, string msg)
        {
            var exist = allUser.FirstOrDefault(m => m.ConnectionId == toId);
            if (exist == null)
            {
                Clients.Caller.addMsg("系统管理员", "该用户已下线");
                return allUser;
            }
            if (exist.ConnectionId != Context.ConnectionId)
            {
                //存在那就发送
                Clients.Client(toId).addMsg(fromName, msg);
                Clients.Caller.addMsg(fromName, msg);
            }
            else
            {
                Clients.Client(toId).addMsg(fromName, msg);
            }
            return allUser;
        }

        public void FirstConnection(string name)
        {
            allUser.Add(new UserMsg
            {
                ConnectionId = Context.ConnectionId,
                Name = name
            });
            Clients.All.AddUserList(allUser);
            return;
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            allUser = allUser.FindAll(m => m.ConnectionId != Context.ConnectionId);
            Clients.All.AddUserList(allUser);
            return base.OnDisconnected(stopCalled);
        }

    }
}