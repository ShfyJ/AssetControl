using System;

namespace ControlActive.ViewModels
{
    public class RealEstateShortDto
    {
        public int RealEstateId { get; set; }
        public string RealEstateName { get; set; }
        public string CadastreNumber { get; set; }
        public string CadastreRegDate { get; set; }
        public string CommisioningDate { get; set; }
        public string Activity { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string AssetHolderName { get; set; }
        public string CadastreFileLink { get; internal set; }
        public string Photo1Link { get; internal set; }
        public string Photo2Link { get; internal set; }
        public string Photo3Link { get; internal set; }
    }
}
