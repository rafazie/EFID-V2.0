using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_Email
    {
        [Key]
        public int ID { get; set; }
        public string? NoFID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Jabatan { get; set; }
        public string? Status { get; set; }
    }
}
