using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class UserDTO
    {
        public bool Status { get; set; }
        public string Code { get; set; }
        public string Messages { get; set; }
        public DataUser Data { get; set; }
    }

    public class DataUser
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
        public object BankName { get; set; }
        public object NoRekening { get; set; }
    }


    public class ListKodeUnit
    {
        public bool Status { get; set; }
        public string Code { get; set; }
        public string Messages { get; set; }
        public List<ListUser> Data { get; set; }
    }

    public class ListUser
    {
        public string Username { get; set; }
        public string NamaPegawai { get; set; }
        public string NIP { get; set; }
        public string Posisi { get; set; }
        public string NamaJabatan { get; set; }
        public string Fungsi { get; set; }
        public string SalesArea { get; set; }
        public string Email { get; set; }
    }


}