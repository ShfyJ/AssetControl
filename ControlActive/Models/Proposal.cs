using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    //Предложения по дальнейшему эффективному использованию актива
    public class Proposal
    {
        [Key]
        public int ProposalId { get; set; }
        public string ProposalName { get; set; }
        public bool Status { get; set; }
        public ICollection<RealEstate> realEstates { get; set; }
    }
}