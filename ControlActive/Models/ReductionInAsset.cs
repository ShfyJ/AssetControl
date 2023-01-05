using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //6. Блок «Пошаговое снижение стоимости актива» 
    public class ReductionInAsset
    {
        [Key]
        public int ReductionInAssetId { get; set; }
        public bool Confirmed { get; set; }

        #region Орган управления, принявший решение о снижении рыночной стоимости актива, дата и номер решения;
        public string GoverningBodyName { get; set; }                           //Наименование органа управления
        public string SolutionNumber { get; set; }                              //Номер решения
        
        [DataType(DataType.Date)]
        public DateTime SolutionDate { get; set; }                              //Дата решения

        #nullable enable
        public int? SolutionFileId { get; set; }
        public string? SolutionFileLink { get; set; }
        #nullable disable
        public FileModel SolutionFile { get; set; }                             //Решение органа управления о применении пошагового снижения стоимости актива в формате PDF
        

        #endregion

        #region Шаг снижения (%, сумма) 
        [Required]
        public string Percentage { get; set; }                                   // %
        public string Amount { get; set; }                                       //Сумма

        #endregion

        public int NumberOfSteps { get; set; }                                  //Количесво примененных шагов
        public string AssetValueAfterDecline { get; set; }                       //Стоимость актива после снижения, тыс сум

        public bool Status { get; set; }                                        //True(aktivniy)/false(neaktivniy)
        
        [DataType(DataType.Date)]
        public DateTime StatusChangedDate { get; set; }                         

        #nullable enable
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public RealEstate? RealEstate { get; set; }

        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public Share? Share { get; set; }
        #nullable disable

        [NotMapped]
        public string SolutionDateStr { get; set; }
    }
}
