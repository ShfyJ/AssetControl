using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    public class RealEstateTechnicalCharcs
    {
        public int RealEstateId { get; set; }
        public RealEstate RealEstate { get; set; }

        public int TechnicalCharcId { get; set; }
        public TechnicalCharc TechnicalCharc { get; set; }
    }
}
