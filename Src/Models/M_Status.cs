using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_Status
    {
        [Key]
        public int statusID { get; set; }
        public int status { get; set; }
        public string? value { get; set; }
        public string? SlaDefault { get; set; }
    }
}
