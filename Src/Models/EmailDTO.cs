using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class EmailDTO
    {
        public EmailDTO()
        {
            attachments = null;
        }
        public string subject { get; set; }
        public List<string> to { get; set; }
        public List<string> cc { get; set; }
        public string bodyEmailHTML { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Attachment
    {
        public string fileName { get; set; }
        public string fileByte { get; set; }
    }

    public class sampless
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class ResponseManager
    {
        public bool Status { get; set; }
        public string Code { get; set; }
        public string Messages { get; set; }
        public List<ListUserManager> Data { get; set; }
    }

    public class ListUserManager
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string NIP { get; set; }
        public string NamaPegawai { get; set; }
        public string NamaJabatan { get; set; }
        public string Posisi { get; set; }
        public string Fungsi { get; set; }
        public string SalesArea { get; set; }
        public string Email { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string NoRekening { get; set; }
    }

}
