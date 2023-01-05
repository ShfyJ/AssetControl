using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlActive.Models
{
    //5. Блок «Выставление на торги» 
    public class SubmissionOnBidding
    {
        [Key]
        public int SubmissionOnBiddingId { get; set; }
        public bool Confirmed { get; set; }
        public string TradingPlatformName { get; set; }                 //Наименование торговой площадки, на которую выставлен актив
        public string Url { get; set; }                                 //Online Auktsion o'tkaziladigan sayt ssilkasi
        public int AmountOnBidding { get; set; }                        //Количество выставлений на торги

        #region Дата, срок и стоимость выставления на торги
       
        [DataType(DataType.Date)]
        public DateTime BiddingExposureDate { get; set; }               //Дата выставления на торги
        public int ExposureTime { get; set; }                           //Срок выставления
        public string ActiveValue { get; set; }                          //Стоимость актива, тыс сум
       
        [DataType(DataType.Date)]
        public DateTime BiddingHoldDate { get; set; }                   //Дата проведения торгов

        #endregion

        [NotMapped]
        public int DaysLeft { get
            {
                return (BiddingHoldDate - DateTime.Now.Date).Days;    
            } }

        public string Status { get; set; }                                //Сотилди/Сотилмади/Сотувда
        public bool IsActiveForPriceReduction { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime AuctionCancelledDate { get; set; }                //

        #nullable enable
        public int? RealEstateId { get; set; }
        [ForeignKey("RealEstateId")]
        public RealEstate? RealEstate { get; set; }
        
        public int? ShareId { get; set; }
        [ForeignKey("ShareId")]
        public Share? Share { get; set; }
        #nullable disable

        [NotMapped]
        public string BiddingExposureDateStr { get; set; }
        [NotMapped]
        public string BiddingHoldDateStr { get; set; }    
    }
}
