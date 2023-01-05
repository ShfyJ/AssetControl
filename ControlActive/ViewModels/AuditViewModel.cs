using System;

namespace ControlActive.ViewModels
{
    public class AuditViewModel
    {
        public string ActionName { get; set; }
        public string UserFullName { get; set; }
        public string TableName { get; set; }
        public string AssetName { get; set; } = "";
        public string DateTime { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string AffectedColumn { get; set; }
        public string PrimaryKey { get; set; } = "";
    }
}
