using ControlActive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.ViewModels
{
    public class GeneralViewModel
    {
#nullable enable
        public TransferredAsset? TransferredAsset { get; set; }

        public AssetEvaluation? AssetEvaluation { get; set; }

        public SubmissionOnBidding? SubmissionOnBidding { get; set; }   
        public ReductionInAsset? ReductionInAsset { get; set; } 
        public OneTimePaymentAsset? OneTimePaymentAsset { get; set; }
        public OneTimePaymentStep2? OneTimePaymentStep2 { get; set; }
        public OneTimePaymentStep3? OneTimePaymentStep3 { get; set; }
        public InstallmentAsset? InstallmentAsset { get; set; }
        public InstallmentStep2? InstallmentStep2 { get; set; }
        public RealEstate? RealEstate { get; set; }
        public Share? Share { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public int? Target {get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }

        #nullable disable
    }
}
