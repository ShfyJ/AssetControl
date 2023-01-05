using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //Блок «Активы, реализованные в рассрочку» ЭТАП 2
    public class InstallmentStep2
    {
        [Key]
        public int InstallmentStep2Id { get; set; }

        public bool Confirmed { get; set; }

        #region Дата и номер акта прием-передачи актива

        [DataType(DataType.Date)]
        public DateTime ActAndAssetDate { get; set; }                           //Дата акта приём-передачи актива
        public string ActAndAssetNumber { get; set; }                           //Номер акта приём-передачи актива

        #nullable enable
        public string? ActAndAssetFileLink { get; set; }
        public int? ActAndAssetFileId { get; set; }                             //Акт приём-передачи актива в формате PDF
        #nullable disable

        #endregion
        public int InstallmentAssetId { get; set; }
        [ForeignKey("InstallmentAssetId")]
        public InstallmentAsset InstallmentAsset { get; set; }
        public FileModel FileModel { get; set; }
        
        [NotMapped]
        public string ActAndAssetDateStr { get; set; }
    }
}
