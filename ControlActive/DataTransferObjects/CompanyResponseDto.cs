using System;

namespace ControlActive.DataTransferObjects
{
    public class CompanyResponseDto
    {
        public int requstId { get; set; }
        public int responseId { get; set; }
        public string cad_number { get; set; }
        public int count_objects { get; set; }
        public string[] objects { get; set; }
        public int region_id { get; set; }
        public string region { get; set; }
        public int district_id { get; set; }
        public string district { get; set; }
        public float area { get; set; }
        public string address { get; set; }

        public string cost { get; set; }
        public DateTime date { get; set; }
        public string number { get; set; }
        public int count_subjects { get; set; }
        public Subjects[] subjects { get; set; }
        public string docs { get; set; }
        public string right { get; set; }
        public string purpose { get; set; }
        public int object_type { get; set; }
        public int has_ban { get; set; }
        public string ban_info { get; set; }
        public int code { get; set; }
        public int time { get; set; }
        public string message { get; set; }

        public string dom_num { get; set; }
        public string kvartira_num { get; set; }
        public string neighborhood { get; set; }
        public string neighborhood_id { get; set; }

        public string exceptionMessage { get; set; }


    }

  
}
