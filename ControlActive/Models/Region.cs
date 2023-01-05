
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    public class Region
    {
        [Key]
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public bool Status { get; set; }
        public string AmchartMapId { get; set; }
        public ICollection<District> Districts { get; set; }
    }
}