using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_Fungsi
    {
        [Key]
        public int ID { get; set; }
        public string? FungsiID { get; set; }
        public string? FungsiName { get; set; }
    }
}
