using ControlActive.Models;

namespace ControlActive.ViewModels
{
    public class UserViewModel
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string OrganizationName { get; set; }
        public int RealEstateCount { get; set; }
        public int ShareCount { get; set; }
        public int SoldAssetCount { get; set; }
    }
}
