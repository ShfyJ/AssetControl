namespace ControlActive.Models
{
    public class SharesAndHolders
    {
        public int ShareId { get; set; }
        public Share Share { get; set; }

        public int ShareholderId { get; set; }
        public Shareholder Shareholder { get; set; }
    }
}
