using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class T_Catatan
    {
        [Key]
        public int CatatanID { get; set; }
        public string? NoFID { get; set; }
        public string? Catatan { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUser { get; set; }
    }
}
