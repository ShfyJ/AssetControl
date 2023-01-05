using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    public class RealEstateInfrastructure
    {
        public int RealEstateId { get; set; }
        public RealEstate RealEstate { get; set; }

        public int InfrastuctureId {get; set;}
        public Infrastucture Infrastucture { get; set; }
    }
}
