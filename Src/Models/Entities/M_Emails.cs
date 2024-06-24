using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models.Entities
{
    public partial class M_Email
    {
        [Key]
        public int ID { get; set; }
        public string NoFID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Jabatan { get; set; }
        public string Status { get; set; }
    }

    public partial class M_EmailManager
    {
        [Key]
        public int Id { get; set; }
        public string Jabatan { get; set; }
        public string Email { get; set; }
    }

    public partial class M_Fungsi
    {
        [Key]
        public int ID { get; set; }
        public string FungsiID { get; set; }
        public string FungsiName { get; set; }
    }

    public partial class M_Status
    {
        [Key]
        public int statusID { get; set; }
        public Nullable<short> status { get; set; }
        public string value { get; set; }
        public string SlaDefault { get; set; }
    }

    public partial class M_TemplateForm
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string HtmlTag { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public string CreateBy { get; set; }
    }

    public partial class M_UploadFiles
    {
        [Key]
        public int UploadID { get; set; }
        public string NoFID { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Key4 { get; set; }
        public string UploadBy { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<short> Status { get; set; }
    }

    public partial class M_User
    {
        [Key]
        public int userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public Nullable<int> flag { get; set; }
    }

    public partial class T_Catatan
    {
        [Key]
        public int CatatanID { get; set; }
        public string NoFID { get; set; }
        public string Catatan { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedUser { get; set; }
    }

    public partial class T_FID
    {
        [Key]
        public int DocID { get; set; }
        public string NoFID { get; set; }
        public string Judulfid { get; set; }
        public string TahunAnggaran { get; set; }
        public Nullable<decimal> NilaiABI { get; set; }
        public string Jenisfid { get; set; }
        public string Fungsipengusul { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<short> Status { get; set; }
    }

    public partial class vFID
    {
        [Key]
        public int DocID { get; set; }
        public string Status { get; set; }
        public string NoFID { get; set; }
        public string Judulfid { get; set; }
        public string TahunAnggaran { get; set; }
        public Nullable<decimal> NilaiABI { get; set; }
        public string Jenisfid { get; set; }
        public string FungsiID { get; set; }
        public string Fungsipengusul { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<short> sts { get; set; }
        public string SlaDefault { get; set; }
        public string SlaRealisasi { get; set; }
        public Nullable<int> OverDays { get; set; }
    }

    public partial class T_DocFID
    {
        [Key]
        public int Id { get; set; }
        public string Category { get; set; }
        public string NoFID { get; set; }
        public string FileString { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}