using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //2. Блок «Общие сведения» (для долей)
    public class Share
    {
        [Key]
        public int ShareId { get; set; }

        [Required]
        [Display(Name = "Наименование хозяйствующего субъекта")]
        public string BusinessEntityName { get; set; }                              //Наименование хозяйствующего субъекта

        public bool Status { get; set; }                                            //obyekt UNG hisobida(true) yoki hisobdan chiqarildi(false)

        public bool Confirmed { get; set; }                                         //объект маълумотлари фойдаланувчи тарафидан тасдиқланган

        [DataType(DataType.Date)]
        public DateTime OutOfAccountDate { get; set; }                              //obyekt ro'yxatdan chiqarilgan sana

        [Required] 
        [Display(Name = "Идентификационный реестровый номер (ИНН)")]
        public string IdRegNumber { get; set; }                                     //Идентификационный реестровый номер
        public string ParentOrganization { get; set; }                              //Вышестоящая организация либо отраслевая принадлежность

        public string Activities { get; set; }                                      //Основной вид деятельности хозяйствующего субъекта

        [Required]
        [Range(0, 100, ErrorMessage = "Пожалуйста, введите действительное значение (0-100)")]
        [Display(Name = "Доля от основного вида деятельности, %")]
        public string ActivityShare { get; set; }                                    //Доля от основного вида деятельности, %

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Год создания организации")]
        public DateTime FoundationYear { get; set; }                                //Год создания организации

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата государственной регистрации")]
        public DateTime StateRegistrationDate { get; set; }                         //Дата государственной регистрации
        public string RegCertificateLink { get; set; }
        public int RegCertificateId { get; set; }                                   //Свидетельство о государственной регистрации  в формате PDF 
        public string OrgCharterLink { get; set; }
        public int OrgCharterId { get; set; }                                       //Устав организации в формате PDF

        public ICollection<FileModel> Files { get; set; }
        public ICollection<Shareholder> Shareholders { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }                //9. Блок «Ответственное лицо по заполнению данных» 
        
        #nullable enable
        public int? TransferredAssetId { get; set; }
        [ForeignKey("TransferredAssetId")]
        public virtual TransferredAsset? TransferredAsset { get; set; }             //3. Блок «Переданные активы»      
        
        public ICollection<AssetEvaluation>? AssetEvaluations { get; set; }          //4. Блок «Оценка актива» 

        public ICollection<SubmissionOnBidding>? SubmissionOnBiddings { get; set; }  //5. Блок «Выставление на торги»

        public ICollection<ReductionInAsset>? ReductionInAssets { get; set; }        //6. Блок «Пошаговое снижение стоимости актива»                      

        public ICollection<OneTimePaymentAsset>? OneTimePaymentAssets { get; set; }  //7. Блок «Реализованные активы с единовременной оплатой»

        public ICollection<InstallmentAsset>? InstallmentAssets { get; set; }        //8. Блок «Активы, реализованные в рассрочку» 
        #nullable disable


        [Required]
        public bool TransferredAssetOn { get; set; }
        public bool AssetEvaluationOn { get; set; }
        public bool SubmissionOnBiddingOn { get; set; }
        public bool ReductionInAssetOn { get; set; }
        public bool OneTimePaymentAssetOn { get; set; }
        public bool InstallmentAssetOn { get; set; }

        #region Месторасположение объекта
        public int RegionId { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region RegionOfObject { get; set; }                          //region
        public int DistrictId { get; set; }

        [ForeignKey("DistrictId")]
        public virtual District DistrictOfObject { get; set; }                      //district
        public string Address { get; set; }                                         //адрес (поселок, улица, дом, ССГ, МСГ и т.д.)

        #endregion

        [Required]
        [Display(Name = "Уставный капитал (тыс сум)")]
        [Range(0, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string AuthorizedCapital { get; set; }                                //Уставный капитал (тыс сум)

        #region Сведения об акционере/участнике и размер доли (%, сумма)       
        
        //public string ShareHolderName { get; set; }                                 //акционер/участник

        //[Required]
        //[Display(Name = "Сумма от уставного капитала")]
        ////[Range(0, float.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        //public string AmountFromAuthCapital { get; set; }                            //Сумма от уставного капитала

        //[NotMapped]
        //public string ShareOfAuthCapital
        //{
        //    get
        //    {
        //          string id = "";
        //          foreach(var s1 in AmountFromAuthCapital.Split(';'))
        //        {
        //            if(s1 != " " && s1 != "" && s1 != null)
        //            {
        //       id += "; " + Math.Round((decimal.Parse(s1, CultureInfo.InvariantCulture.NumberFormat) * 100 / decimal.Parse(AuthorizedCapital, CultureInfo.InvariantCulture.NumberFormat)), 2);
        //            }
        //        }
                
        //        return id;
        //    }//Доля от уставного капитала, %  CultureInfo.InvariantCulture.NumberFormat

        //}

        #endregion


        #region Количество акций и номинальная стоимость акции (для АО)

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Количество акций (штук)")]
        public int NumberOfShares { get; set; }                                     //Количество акций (штук)

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Номинальная стоимость акций, сум")]
        public float ParValueOfShares { get; set; }                                 //Номинальная стоимость акций, сум

        #endregion

        #region Среднесписочная численность работников всего, административный персонал, производственный персонал

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Количество административный персонал")]
        public int AdministrativeStaff { get; set; }                                //административный персонал

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Количество производственный персонал")]
        public int ProductionPersonal { get; set; }                                 //производственный персонал

        [NotMapped]
        public int AllStaff 
        {
            get { return AdministrativeStaff + ProductionPersonal; }
         
        }

        #endregion

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        [Display(Name = "Среднемесячная заработная плата, (тыс сум)")]
        public string AverageMonthlySalary { get; set; }                             //Среднемесячная заработная плата, (сум)

        [Required]
        public string MaintanenceCostForYear { get; set; }                           ////o'rtacha yillik saqlash xarajati

        #region Общая площадь (Га), из них производственные площади (кв.м.), здания сооружения (кв.м.)

        [Required]
        [Display(Name = "Производственная площадь (кв.м)")]
        [Range(0, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string ProductionArea { get; set; }                                  //Производственная площадь, кв.м

        [Required]
        [Display(Name = "Здание сооружения (кв.м)")]
        [Range(0, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string BuildingsArea { get; set; }                                   //здания сооружения (кв.м.)

        [NotMapped]
        public float AllArea
        {
            get { return (float)((float.Parse(BuildingsArea, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(ProductionArea, CultureInfo.InvariantCulture.NumberFormat)) / Math.Pow(10, 4)); }
        }

        #endregion

        [Required]
        [Display(Name = "Размер кредиторской задолженности по состоянию на последнюю отчетную дату, тыс сум")]
        public string AmountPayable { get; set; }                                    //Размер кредиторской задолженности по состоянию на последнюю отчетную дату, сум

        [Required]
        [Display(Name = "Размер дебиторской задолженности по состоянию на последнюю отчетную дату, тыс сум")]
        public string AmountReceivable { get; set; }                                 //Размер дебиторской задолженности по состоянию на последнюю отчетную дату, сум

        #region Финансовый результат (прибыль/ убыток за последние три года), тыс сум

        [Required]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string ProfitOrLossOfYear1 { get; set; }                              //Прибыль(+)/убыток(-) за последний год 
        [Required]
        public string Year1 { get; set; }

        [Required]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string ProfitOrLossOfYear2 { get; set; }                              //Прибыль(+)/убыток(-) за перед последний год
        [Required]
        public string Year2 { get; set; }

        [Required]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Пожалуйста, введите действительное значение")]
        public string ProfitOrLossOfYear3 { get; set; }                              //Прибыль(+)/убыток(-) за перед (перед) последний год
        [Required]
        public string Year3 { get; set; }

        #endregion
        public string Comments { get; set; }

        public int BalanceSheetId { get; set; }                                     //Бугалтерский баланс
        public string BalanceSheetLink { get; set; }
        public int FinancialResultId { get; set; }                                  //отчет финансовый ресултат
        public string FinancialResultLink { get; set; }

        public int AuditConclusionId { get; set; }                                    //Аудиторлик хулосаси      
        public string AuditConclusionLink { get; set; } 

        [NotMapped]
        public string FoundationYearStr { get; set; }
        [NotMapped]
        public string StateRegistrationDateStr { get; set; }

        //o'rtacha yillik saqlash xarajati qo'shilishi kerak

    }
}
