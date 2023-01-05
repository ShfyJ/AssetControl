using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlActive.Models
{
    /* Форма передачи  
    - безвозмездная;
    - в счет уменьшения доли;
    - в счет задолженности;
    - вклад в уставный капитал;
    - мена
    */
    public class TransferForm
    {
        [Key]
        public int TransferFormId { get; set; }
        public string TransferFormName { get; set; }
        public bool Status { get; set; }
        public ICollection<TransferredAsset> TransferredAssets { get; set; }

    }
}