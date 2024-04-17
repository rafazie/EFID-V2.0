using System.ComponentModel.DataAnnotations;

namespace EFID_V2.Models
{
    public class vFID
    {
        public string? Status { get; set; }
        [Key]
        public int DocID { get; set; }
        public string? NoFID { get; set; }
        public string? Judulfid { get; set; }
        public string? TahunAnggaran { get; set; }
        public decimal? NilaiABI { get; set; }
        public string? Jenisfid { get; set; }
        public string? FungsiID { get; set; }
        public string? Fungsipengusul { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public short? sts { get; set; }
        public string? SlaDefault { get; set; }
        public string? SlaRealisasi { get; set; }
        public int?  OverDays { get; set; }
    }
}
