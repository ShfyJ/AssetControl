using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    public class Shareholder
    {
        [Key]
        public int ShareholderId { get; set; }
        public string ShareholderName { get; set; }
        public string AmountFromAuthCapital { get; set; }

        public ICollection<Share> Shares { get; set; }
    }
}
