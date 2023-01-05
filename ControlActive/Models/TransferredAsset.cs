using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //3. Блок «Переданные активы» 
    public class TransferredAsset
    {
        [Key]
        public int AssetId { get; set; }

        public bool Confirmed { get; set; }
        public int TransferFormId { get; set; }
        [ForeignKey("TransferFormId")]
        public TransferForm TransferForm { get; set; }                          //Форма передачи
        #nullable enable
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public RealEstate? RealEstate { get; set; }
        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public Share? Share { get; set; }
        #nullable disable
        public ICollection<FileModel> Files { get; set; }

        #region Орган управления, принявший решение о передаче актива 

        public string OrgName { get; set; }                                     //Орган управления, принявший решение о передаче актива 
        public string SolutionNumber { get; set; }                              //Номер решения

        [Required]
        [DataType(DataType.Date)]
        public DateTime SolutionDate { get; set; }                              //Дата решения
        public string SolutionFileLink { get; set; }
        public int SolutionFileId { get; set; }                                 //Решение органа управления в формате PDF
        #endregion

        public string OrgNameOfAsset { get; set; }                              //Наименование организации, которой передан актив

        #region Стоимость передачи актива, в том числе НДС
        public string TotalCost { get; set; }                                    //Общая стоимость, тыс сум
        public double VAT
        {
            get { return float.Parse(TotalCost, CultureInfo.InvariantCulture.NumberFormat) * 0.15; }                                    //НДС, тыс сум
        }
        #endregion

        #region Дата и номер акта приём-передачи актива
        [Required]
        [DataType(DataType.Date)]
        public DateTime ActAndAssetDate { get; set; }              //Дата акта приём-передачи актива
        public string ActAndAssetNumber { get; set; }              //Номер акта приём-передачи актива
        public string ActAndAssetFileLink { get; set; }
        public int ActAndAssetFileId { get; set; }                 //Акт приём-передачи актива в формате PDF

        #endregion

        #region Дата и номер договора передачи актива
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime AgreementDate { get; set; }                             //Дата  договора передачи актива
        public string AgreementNumber { get; set; }                             //Номер договора передачи актива

        public string AggreementFileLink { get; set; }
        public int AgreementFileId { get; set; }                                //договор передачи актива в формате PDF


        #endregion

        [NotMapped]
        public string SolutionDateStr { get; set; }
        [NotMapped]
        public string ActAndAssetDateStr { get; set; }
        [NotMapped]
        public string AgreementDateStr { get; set; }
    }

}
