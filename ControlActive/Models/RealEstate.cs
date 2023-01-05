using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //1. Блок «Общие сведения» (для объектов недвижимости) 
    public class RealEstate
    {
        [Key]
        public int RealEstateId { get; set; }
        
        [Required]
        [Display(Name = "Наименование объекта")]
        public string RealEstateName { get; set; }                                  //Наименование объекта (по кадастру)

        public bool Status { get; set; }                                            //obyekt UNG hisobida(true) yoki hisobdan chiqarildi(false)

        public bool Confirmed { get; set; }                                         //объект маълумотлари фойдаланувчи тарафидан тасдиқланган
        //public bool OnRequestForEdit { get; set; }

        [DataType(DataType.Date)]
        public DateTime OutOfAccountDate { get; set; }                              //obyekt ro'yxatdan chiqarilgan sana

        [DataType(DataType.Date)]
        public DateTime Date_Added { get; set; }                                    

        [Required]
        [StringLength(20, ErrorMessage = "Кадастр рақами 14 белгидан иборат!", MinimumLength = 12)]
        [Display(Name = "Идентификатсия регистр рақами (кадастр рақами)")]
        public string CadastreNumber { get; set; }                                  //14 simvol
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name= "Кадастрни рўйхатдан ўтказиш санаси")]
        public DateTime CadastreRegDate { get; set; }                               //Дата регистрации кадастра
        
        public string CadastreFileLink { get; set; }
        public int CadastreFileId { get; set; }                                     //cadastre file (only PDF)

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ишга тушириш санаси")]
        public DateTime CommisioningDate { get; set; }                              //Дата ввода в эксплуатацию

        [Required]
        [Display(Name = "Наименование вида деятельности")]
        public string Activity { get; set; }                                        //Наименование вида деятельности
      
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }                //9. Блок «Ответственное лицо по заполнению данных» 

    
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
        [Display(Name = "Наименование балансодержателя")]
        public string AssetHolderName { get; set; }                                 //Наименование балансодержателя

        #region Общая площадь (Га), из них производственные площади (кв.м.), здания сооружения (кв.м.)

        [Required]
        [Display(Name = "Бино иншоот майдони (кв.м)")]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public string BuildingArea { get; set; }                                     //Здание сооружения (кв.м.) Bino inshoot maydoni
        
        [Required]
        public string FullArea { get; set; }

        #endregion

        public ICollection<Infrastucture> Infrastructures { get; set; }             //Наличие инфраструктуры
       
        [NotMapped]
        public string InfrastructureNames { get; set; }

        [Required]
        [Display(Name = "Ходимлар сони")]
        [Range(0, int.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public int NumberOfEmployee { get; set; }                                   //Количество работников

        public string PhotoOfObjectLink1 { get; set; }
        public int PhotoOfObject1Id { get; set; }                                  //Фото1 объекта Только в формате JPG

        public string PhotoOfObjectLink2 { get; set; }
        public int PhotoOfObject2Id { get; set; }                                  //Фото2 объекта Только в формате JPG

        public string PhotoOfObjectLink3 { get; set; }
        public int PhotoOfObject3Id { get; set; }                                  //Фото3 объекта Только в формате JPG

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public string MaintenanceCostForYear { get; set; }                          //o'rtacha yillik saqlash xarajati      //Расходы на содержание за отчетный год (тыс сум)
        

        [Required]
        [Display(Name = "Первоначальная стоимость объекта (тыс сум)")]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public string InitialCostOfObject { get; set; }                              //Первоначальная стоимость объекта (тыс сум)

        [Required]
        [Display(Name = "Износ (тыс сум)")]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public string Wear { get; set; }                                             //Износ (тыс сум) : amortizatsiya

        [Required]
        [Display(Name = "Балансовая (остаточная) стоимость объекта (тыс сум)")]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, тўғри маълумот киритинг")]
        public string ResidualValueOfObject { get; set; }                            //Балансовая (остаточная) стоимость объекта (тыс сум)
        
        public int ProposalId { get; set; }

        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }                              //Предложения по дальнейшему эффективному использованию актива
        
        public string Comment { get; set; }                                         //Комментарии

        public ICollection<FileModel> Files { get; set; }

        [Required]
        public bool TransferredAssetOn { get; set; }
        public bool AssetEvaluationOn { get; set; }
        public bool SubmissionOnBiddingOn { get; set; }
        public bool ReductionInAssetOn { get; set; }
        public bool OneTimePaymentAssetOn { get; set; }
        public bool InstallmentAssetOn { get; set; }

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

        [NotMapped]
        public string CadasterRegDateStr { get; set; }
        [NotMapped]
        public string CommisioningDateStr { get; set; }
    }
}
