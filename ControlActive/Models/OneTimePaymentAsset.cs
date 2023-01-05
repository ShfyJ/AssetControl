using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //7. Блок «Реализованные активы с единовременной оплатой» Этап (1)
    public class OneTimePaymentAsset
    {
        [Key]
        public int OneTimePaymentAssetId { get; set; }

        #region Орган управления, принявший решение о реализации актива, дата и номер решения;
        public string GoverningBodyName { get; set; }                           //Наименование органа управления
        public string SolutionNumber { get; set; }                              //Номер решения
        public bool Confirmed { get; set; }

        [DataType(DataType.Date)]
        public DateTime SolutionDate { get; set; }                              //Дата решения
        public string SolutionFileLink { get; set; }
        public int SolutionFileId { get; set; }                                 //Решение органа управления о реализации актива в формате PDF
        #endregion
        [DataType(DataType.Date)]
        public DateTime BiddingDate { get; set; }                               //Дата проведения торгов (реализации актива)

        public FileModel FileModel { get; set; }

        public string Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime BiddingCancelledDate { get; set; }

        #nullable enable
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public RealEstate? RealEstate { get; set; }

        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public Share? Share { get; set; }

        public int? OneTimePaymentStep2Id { get; set; }
        [ForeignKey("OneTimePaymentStep2Id")]
        public OneTimePaymentStep2? Step2 { get; set; }

        public int? OneTimePaymentStep3Id { get; set; }
        [ForeignKey("OneTimePaymentStep3Id")]
        public OneTimePaymentStep3? Step3 { get; set; }
        #nullable disable

        [NotMapped]
        public string SolutionDateStr { get; set; }
        [NotMapped]
        public string BiddingDateStr { get; set; }




    }
}
