using System.Collections.Generic;

namespace ControlActive.ViewModels
{
    public class OrganizationViewModel
    {
        public string name { get; set; }
        public Children[]children { get; set; }
        public class Children
        {
            public string name { get; set; }
            public List<SmallChildren>children { get; set; }
            public string type { get; set; }
        }

        public class SmallChildren
        {
            public string name { get; set; }
            public int value { get { return 1; } }
        }

    }
}
