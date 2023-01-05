using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    public class NotiMessage
    {
        [Key]
        public int NotiMessageId { get; set; }
        public string Content { get; set; }
        public int MessageType { get; set; }
        
    }
}
