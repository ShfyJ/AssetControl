using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //«Реализованные активы с единовременной оплатой» Акт приём-передачи Этап (3)
    public class OneTimePaymentStep3
    {
        [Key]
        public int OneTimePaymentStep3Id { get; set; }

        public bool Confirmed { get; set; }

        #region Акт приема-передачи актива
        [DataType(DataType.Date)]
        public DateTime ActAndAssetDate { get; set; }              //Дата акта приём-передачи актива
        public string ActAndAssetNumber { get; set; }              //Номер акта приём-передачи актива
        public string ActAndAssetFileLink { get; set; }
        public int ActAndAssetFileId { get; set; }                 //Акт приём-передачи актива в формате PDF

        public string InvoiceFileLink { get; set; }                //Ҳисобғварақ фактура в формате PDF
        public int InvoiceFileId { get; set; }
        #endregion
        public int OneTimePaymentAssetId { get; set; }
        [ForeignKey("OneTimePaymentAssetId")]
        public OneTimePaymentAsset OneTimePaymentAsset { get; set; }
        public FileModel FileModel { get; set; }

        [NotMapped]
        public string ActAndAssetDateStr { get; set; }
    }
}
