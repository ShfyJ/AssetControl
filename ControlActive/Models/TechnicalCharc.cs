using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    //Техническая характеристика здания
    public class TechnicalCharc
    {
        [Key]
        public int TechnicalCharcId { get; set; }
        public string TechnicalCharcName { get; set; }
        public bool Status { get; set; }
        public ICollection<RealEstate> RealEstate { get; set; }
    }
}