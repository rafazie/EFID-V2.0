using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class M_UploadFiles
    {
        [Key]
        public int UploadID { get; set; }
        public string? NoFID { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public string? Key4 { get; set; }
        public string? UploadBy { get; set; }
        public DateTime? UploadDate { get; set; }
        public int Status { get; set; }
    }
}
