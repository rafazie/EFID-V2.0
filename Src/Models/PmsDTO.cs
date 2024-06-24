using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class BodyFID
    {
        public string jenis_investasi { get; set; }
        public DateTime? date_fid { get; set; }
    }

    public class ResponseFID
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public List<datas> Data { get; set; }
    }

    public class datas
    {
        public string NoFID { get; set; }
        public string Judulfid { get; set; }
        public string FungsiPengusul { get; set; }
        public string JenisInvestasi { get; set; }
        public string TahunAnggaran { get; set; }
        public Nullable<decimal> NilaiABI { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter), "yyyy-MM-dd")]
        public string CreatedDate { get; set; }
    }

    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd-MMMM-yyyy";
        }
    }

    public class ResponseFID2
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public datas Data { get; set; }
        public List<dataFiles> files { get; set; }
    }

    public class dataFiles
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Files { get; set; }
    }

    public static class AvoidNull
    {
        public static string AvoidNullString(this string value)
        {
            if (value == null)
                return "";
            return value;
        }
    }

}

