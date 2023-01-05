using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //4. Блок «Оценка актива» 
    public class AssetEvaluation
    {
        [Key]
        public int AssetEvaluationId { get; set; }
        public string EvaluatingOrgName { get; set; }               //Наименование оценочной организации
        public bool Confirmed { get; set; }                         

        #region Дата и регистрационный номер отчета об оценке
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }                    //Дата отчета об оценке
        public string ReportRegNumber { get; set; }                 //Регистрационный номер отчета об оценке
        public string ReportFileLink { get; set; }
        public int ReportFileId { get; set; }                       //Отчет об оценке в формате PDF

        #endregion

        public string MarketValue { get; set; }                      //Рыночная стоимость актива, тыс сум

        #region Экспертиза оценки
        public string ExaminingOrgName { get; set; }                //Наименование организации по проведению экспертизы отчета об оценке
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExamReportDate { get; set; }                //Дата отчета об экспертизе оценки
        public string ExamReportRegNumber { get; set; }             //Регистрационный номер отчета об экспертизе оценки
        public string ExamReportFileLink { get; set; }
        public int ExamReportFileId { get; set; }                   //Отчет об экспертизе в формате PDF
        public bool ReportStatus { get; set; }                      //Статус отчета об оценке достоверна(True)/недостоверна(False)
        [NotMapped]
        public string ReportS { get 
            { if (ReportStatus) return "Ишончли";
                else return "Ишончсиз";
            }
        }
        #endregion

        public bool Status { get; set; }                            //status otsenka: Aktivnaya/Neoktivnaya

        [DataType(DataType.Date)]
        public DateTime StatusChangedDate { get; set; }             //Status ўзгарган вақт

        public ICollection<FileModel> Files { get; set; }

        #nullable enable
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public RealEstate? RealEstate { get; set; }

        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public Share? Share { get; set; }
        #nullable disable

        [NotMapped]
        public string ReportDateStr { get; set; }
        [NotMapped]
        public string ExamReportDateStr { get; set; }
        [NotMapped]
        public string ReportStatusStr { get; set; }
    }
}
