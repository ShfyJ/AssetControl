using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlActive.Models
{
    public class Template
    {
        [Key]
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public bool IsActive { get; set; }
        public bool IsShare { get; set; }
        public bool IsRealEstate { get; set; }
        public bool HasUser { get; set; }
        public bool HasTransferredAssets { get; set; }
        public bool HasAssetEvaluation { get; set; }
        public bool HasAuction { get; set; }
        public bool HasReductionInAsset { get; set; }
        public bool HasOneTimePaymentAsset { get; set; }
        public bool HasInstallmentAsset { get; set; }

        [Required]
        public int FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual FileModel File { get; set; }

    }
}
