using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //9. Блок «Ответственное лицо по заполнению данных» 

    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        #nullable enable
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }
        #nullable disable

        [NotMapped]
        #nullable enable
        public string? Fullname
        {
            get { return LastName + " " + FirstName + " " + MiddleName; }        //Ф.И.О. ответственного лица

        }
        public string? Postion { get; set; }                                      //Должность
        public bool isPasswordRenewed { get; set; }
        public string? CreatedById { get; set; }
        #nullable disable
        public ICollection<RealEstate> RealEstates { get; set; }
        public ICollection<Share> Shares { get; set; }
        public ICollection<FileModel> Files { get; set; }
        
        [NotMapped]
        public string Role { get; set; }
    }
}
