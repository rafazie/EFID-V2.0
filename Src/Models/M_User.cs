using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_User
    {
        [Key]
        public int userid { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public int? flag { get; set; }
    }
}
