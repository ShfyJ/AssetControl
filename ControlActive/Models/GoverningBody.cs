using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    public class GoverningBody
    {
        [Key]
        public int GoverningBodyId { get; set; }
        public string GoverningBodyName { get; set; }
        public bool Status { get; set; }
        
    }
}