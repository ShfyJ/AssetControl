using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //«Реализованные активы с единовременной оплатой» Договор Этап (2)
    public class OneTimePaymentStep2
    {
        [Key]
        public int OneTimePaymentStep2Id { get; set; }
        public string AssetBuyerName { get; set; }                              //Полное наименование покупателя актива

        public bool Confirmed { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, суммани тўғри киритинг")]
        public string AmountOfAssetSold { get; set; }                            //Сумма реализации актива, тыс сум
        public int OneTimePaymentAssetId { get; set; }
        [ForeignKey("OneTimePaymentAssetId")]
        public OneTimePaymentAsset OneTimePaymentAsset{ get; set; }
        public FileModel FileModel { get; set; }

        #region Договор купли-продажи
        [DataType(DataType.Date)]
        public DateTime AggreementDate { get; set; }                            //Дата договора
        public string AggreementNumber { get; set; }                            //Номер договора
        public string AggreementFileLink { get; set; }
        public int AggreementFileId { get; set; }                               //Договор купли-продажи в формате PDF
        #endregion

        #region Фактически оплаченная сумма за реализованный актив (сум, %)

        #nullable enable
        [Range(0, float.MaxValue, ErrorMessage = "Илтимос, суммани тўғри киритинг")]
        public string? AmountPayed { get; set; }                                  //Сумма
        #nullable disable
        public float? Percentage                                                  //Процент от стоимости
        {
            get {

                try {
                    if (AmountPayed != null && AmountOfAssetSold != null)
                        return (float)Math.Round((Convert.ToDouble(AmountPayed) * 100/Convert.ToDouble(AmountOfAssetSold)), 3);
                    
                    else return 0;
                }

                catch
                {
                    return 0;
                }
              }
        }
        [DataType(DataType.DateTime)]
        public DateTime ContractCancelledDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ChangeOnAmountPayedDate { get; set; }
        #endregion

        [NotMapped]
        public string AgreementDateStr { get; set; }
    }
}
