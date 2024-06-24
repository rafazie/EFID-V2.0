using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class PengajuanUsulanDTO
    {
        public string Category { get; set; }
        public string Email { get; set; }
        public List<M_Fungsi> M_Fungsi { get; set; }
    }
}