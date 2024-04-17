using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_EmailManager
    {
        [Key]
        public int Id { get; set; }
        public string? Jabatan { get; set; }
        public string? Email { get; set; }
    }
}
