using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlActive.Models
{
    public class Noti
    {
        [Key]
        public int NotiId { get; set; } = 0;
        public string FromUserId { get; set; } = "";
        public string ToUserId { get; set; } = "";
        public string NotiHeader { get; set; } = "";
        public string NotiBody { get; set; } = "";
        public bool IsRead { get; set; } = false;
        public string Url { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Message { get; set; } = "";
        public short MessageType { get; set; } = 0;
        public string CreatedDateSt => this.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss");
        public string IsReadSt => this.IsRead ? "YES" : "NO";

        public string FromUserName { get; set; } = "";
        public string ToUserName { get; set; } = "";
        public int ObjectId { get; set; } = 0;
        public int ObjectType { get; set; } = 0;
        public bool isPermitted { get; set; } = false;
        
        [NotMapped]
        public string CreatedTimeStr { get; set; }
    }
}