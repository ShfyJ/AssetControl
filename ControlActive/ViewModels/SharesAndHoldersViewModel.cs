using ControlActive.Models;

namespace ControlActive.ViewModels
{
    public class SharesAndHoldersViewModel
    {
        public Share Share { get; set; }

        #nullable enable
        public ApplicationUser? ApplicationUser { get; set; }
        #nullable disable

        public string Shareholders { get; set; }
        public string ShareAmount { get; set; }
        public string SharePercentage { get; set; }
    }
}
