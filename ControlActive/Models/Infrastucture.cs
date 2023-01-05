using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    public class Infrastucture
    {
        [Key]
        public int InfrastructureId { get; set; }
        public string InfrastructureName { get; set; }
        public bool Status { get; set; }
        public ICollection<RealEstate> RealEstates { get; set; }
    }
}