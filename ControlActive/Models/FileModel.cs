using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    public class FileModel
    {
        [Key]
        public int FileId { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        //public string Description { get; set; }
        public string FilePath { get; set; }
        public string SystemPath { get; set; }
        public string BasePath { get; set; }
        public string UploadedById { get; set; }
        [ForeignKey("UploadedById")]
        public virtual ApplicationUser Uploadedby { get; set; }
        public DateTime? CreatedOn { get; set; }

        #nullable enable
        public int? TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public virtual Template? Template { get; set; }
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; set; }
        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public virtual Share? Share { get; set; }
        public int? AssetEvaluationId { get; set; }
        [ForeignKey("AssetEvaluationId")]
        public virtual AssetEvaluation? AssetEvaluation { get; set; }
        public int? InstallmentAssetId { get; set; }
        [ForeignKey("InstallmentAssetId")]
        public virtual InstallmentAsset? InstallmentAsset { get; set; }

        public int? InstallmentStep2Id { get; set; }
        [ForeignKey("InstallmentStep2Id")]
        public virtual InstallmentStep2? InstallmentStep2{ get; set; }

        public int? OneTimePaymentAssetId { get; set; }
        [ForeignKey("OneTimePaymentAssetId")]
        public virtual OneTimePaymentAsset? OneTimePaymentAsset { get; set; }

        public int? OneTimePaymentStep2Id { get; set; }
        [ForeignKey("OneTimePaymentStep2Id")]
        public virtual OneTimePaymentStep2? OneTimePaymentStep2 { get; set; }

        public int? OneTimePaymentStep3Id { get; set; }
        [ForeignKey("OneTimePaymentStep3Id")]
        public virtual OneTimePaymentStep3? OneTimePaymentStep3 { get; set; }

        public int? ReductionInAssetId { get; set; }
        [ForeignKey("ReductionInAssetId")]
        public virtual ReductionInAsset? ReductionInAsset { get; set; }

        public int? TransferredAssetId { get; set; }
        [ForeignKey("TransferredAssetId")]
        public virtual TransferredAsset? TransferredAsset { get; set; }


    }
}
