using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ControlActive.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public NotificationHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Notify(Noti notification)
        {
            var fromUser = await _context.ApplicationUsers.FindAsync(notification.FromUserId);
            if(fromUser != null)
            {
                notification.FromUserName = fromUser.FirstName + " " + fromUser.LastName;
            }
            var toUser = await _context.ApplicationUsers.FindAsync(notification.ToUserId);
            if(toUser != null)
            {
                notification.ToUserName = toUser.FirstName +" "+ toUser.LastName;
            }
            
            NotiMessage notiMessage = new();

            notiMessage = await _context.NotiMessageLibrary.FirstOrDefaultAsync(s => s.MessageType == notification.MessageType);
                                   
            if (notiMessage == null)
            {
                notification.Message = "Хабар мазмуни топилмади. Хабар типи {" + notification.MessageType+"}";
            }
            else
            {
                notification.Message = notiMessage.Content;
            }
           
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            //var users = new string[] {notification.ToUserId, notification.FromUserId};
            await Clients.User(notification.ToUserId).SendAsync("ReceiveMessage", notification);
        }

    }
}
