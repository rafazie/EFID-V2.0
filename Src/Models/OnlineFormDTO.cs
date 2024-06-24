using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class OnlineFormDTO
    {
        public T_FID T_FID { get; set; }
    }

    public class EditFormDTO
    {
        public T_FID T_FID { get; set; }
        public List<M_UploadFiles> M_UploadFiles { get; set; }
    }

    public class resultDocman
    {
        public string Result { get; set; }
    }
}