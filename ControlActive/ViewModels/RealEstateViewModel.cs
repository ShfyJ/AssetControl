using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControlActive.Models;

namespace ControlActive.ViewModels
{
    public class RealEstateViewModel
    {
        #nullable enable
        public RealEstate? RealEstate { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public AssetEvaluation? AssetEvaluation { get; set; }
        public InstallmentAsset? InstallmentAsset { get; set; }
        public InstallmentStep2? InstallmentStep2 { get; set; }
        public OneTimePaymentAsset? OneTimePaymentAsset { get; set; }
        public OneTimePaymentStep2? OneTimePaymentStep2 { get; set; }
        public OneTimePaymentStep3? OneTimePaymentStep3 { get; set; }
        public ReductionInAsset? ReductionInAsset { get; set; }
        public SubmissionOnBidding? SubmissionOnBidding { get; set; }
        public TransferredAsset? TransferredAsset { get; set; }
        #nullable disable
        
    }
}
