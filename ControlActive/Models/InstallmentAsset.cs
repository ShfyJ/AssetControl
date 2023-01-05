using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //8. Блок «Активы, реализованные в рассрочку» 
    public class InstallmentAsset
    {
        [Key]
        public int InstallmentAssetId { get; set; }
        public bool Confirmed { get; set; }

        #region Орган управления, принявший решение о реализации актива в рассрочку, дата и номер решения
        public string GoverningBodyName { get; set; }                           //Орган управления, принявший решение о передаче актива 
        public string SolutionNumber { get; set; }                              //Номер решения                          

        [DataType(DataType.Date)]
        public DateTime SolutionDate { get; set; }                              //Дата решения

        #nullable enable
        public string? SolutionFileLink { get; set; }
        public int? SolutionFileId { get; set; }                                 //Решение органа управления о реализации актива в рассрочку в формате PDF
        #nullable disable

        #endregion

        [DataType(DataType.Date)]
        public DateTime BiddingDate { get; set; }                               //Дата проведения торгов (реализации актива)
        public string AssetBuyerName { get; set; }                              //Полное наименование покупателя актива
        
        [Range(0, float.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Сумма реализации актива, тыс сум")]
        public string AmountOfAssetSold { get; set; }                            //Сумма реализации актива, тыс сум

        #region Договор купли-продажи актива
        
        [DataType(DataType.Date)]
        public DateTime AggreementDate { get; set; }                            //Дата договора
        public string AggreementNumber { get; set; }                            //Номер договора
        #nullable enable
        public string? AggreementFileLink { get; set; }
        public int? AggreementFileId { get; set; }                               //Договор купли-продажи в формате PDF
        #nullable disable
        #endregion

        #region Оплата стоимости актива
        public int InstallmentTime { get; set; }                                //Срок рассрочки, месяц
        public float MinInitialPaymentPercentage { get { return 15; } }         //Размер минимальной первоначальной оплаты, %
        public double MinInitialPaymentAmount                                   //минимальная первоначальная оплата, тыс сум 0.15x
        { get { return float.Parse(AmountOfAssetSold, CultureInfo.InvariantCulture.NumberFormat) * 0.15; } }  
        
        public string ActualInitPayment { get; set; }                            //Фактическая первоначальная оплата, тыс сум
        public float ActualInitPaymentPercentage                               //Фактическая первоначальная оплата,%
        { get { return float.Parse(ActualInitPayment, CultureInfo.InvariantCulture.NumberFormat) 
                    / float.Parse(AmountOfAssetSold, CultureInfo.InvariantCulture.NumberFormat) * 100; } }

        #endregion

        #region График погашения задолженности и суммы оплат по графику
        public int PaymentPeriodType { get; set; }                              //Выбора периода оплаты (Здесь должен быть выбор периода (год(1), квартал(4), месяц(12)))
        public float ScheduledAmount                                            //Сумма по графикc 
        { get 
            {  return (float.Parse(AmountOfAssetSold, CultureInfo.InvariantCulture.NumberFormat)
                    - float.Parse(ActualInitPayment, CultureInfo.InvariantCulture.NumberFormat))/
                    (InstallmentTime * PaymentPeriodType);
            }
        }
        public string ActualPayment { get; set; }                                //Фактическая оплата (сумма которая за последний период оплачена)
        public float Difference
        {
            get { return Math.Abs(ScheduledAmount - float.Parse(ActualPayment, CultureInfo.InvariantCulture.NumberFormat)); }           //Разница
        }
        #endregion

        public short Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime ContractCancelledDate { get; set; }

        #nullable enable
        public int? InstallmentStep2Id { get; set; }
        [ForeignKey("InstallmentStep2Id ")]
        public InstallmentStep2? Step2 { get; set; }
        #nullable disable
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
        public string SolutionDateStr { get; set; }
        [NotMapped]
        public string BiddingDateStr { get; set; }
        [NotMapped]
        public string AggreementDateStr { get; set; }
    }
}
