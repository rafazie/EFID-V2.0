using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplifikasiFID.Models;
using SimplifikasiFID.Classes;
using System.Configuration;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Data;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Globalization;
using System.Data.Common;
using Newtonsoft.Json;
using SimplifikasiFID.ServiceSSO;
using SimplifikasiFID.ServiceDOC;

namespace SimplifikasiFID.Controllers
{
    public class BaseController : Controller
    {
        public static SSOWSSoapClient ws = new SSOWSSoapClient();
        //public bool isAdmin = Roles.IsUserInRole("SYSADMIN");
        public static Dictionary<String, String> currentSessionUnits = new Dictionary<String, String>();

        public string getSessionUnit(string userName)
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                if (!currentSessionUnits.ContainsKey(userName))
                {
                    try
                    {
                        currentSessionUnits.Add(userName, ws.GetUserUnit(userName));
                    }
                    catch
                    {
                        currentSessionUnits[userName] = ws.GetUserUnit(userName);
                    }
                }
                return currentSessionUnits[userName];
            }
            finally
            {
                ws.Close();
            }
        }

        public class SorterData
        {

            public string field;
            public string direction;

        }


        public class SearchConfg
        {

            public string field;
            public string value;

        }

        public class ScrollConfig
        {

            public string cmd;
            public int offset = 0;
            public int limit = 10;
            public List<SorterData> sort;
            public List<SearchConfg> search;


        }

        public SimplyFIDEntities db = new SimplyFIDEntities();

        public decimal parseDecimal(string s)
        {
            return decimal.Parse(s, new CultureInfo("en-US"));
        }

        public double parseDouble(string s)
        {
            return double.Parse(s, new CultureInfo("en-US"));
        }

        public double getHourDiff(DateTime a, DateTime b)
        {
            return b.Subtract(a).TotalHours;
        }

        protected string sqlStr(string s)
        {
            return "'" + s + "'";
        }

        protected string sqlStrLike(string s)
        {
            return sqlStr("%" + s + "%");
        }

        protected string coalesce(string s, string t = "")
        {
            return s == null ? t : s;
        }

        protected ContentResult JsonContent(object result)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            return new ContentResult
            {
                Content = serializer.Serialize(result),
                ContentType = "application/json"
            };
        }

        public string GetMimeType(string fileName)
		{
			string mimeType = "application/unknown";
			string ext = System.IO.Path.GetExtension(fileName).ToLower();
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null)
				mimeType = regKey.GetValue("Content Type").ToString();
			return mimeType;
		}

        protected ContentResult ListData(string request, string defsort, string[] scols, string idcol, string table, string extrafilter = "")
        {
            string sSorter = "";
            string sSearch = "";
            string sField = "";
            string sDirection = "";
            string sOffset = "";
            string sLimit = "";

            if (request != null)
            {
                ScrollConfig scConfig = JsonConvert.DeserializeObject<ScrollConfig>(request);

                if (scConfig.sort != null)
                {
                    sField = scConfig.sort[0].field.ToString();
                    sDirection = scConfig.sort[0].direction.ToString();

                    if (sField != "" && sField != null)
                    {
                        sSorter = sField + " " + sDirection;
                    }
                }

                if (scConfig.search != null)
                {
                    if (scConfig.search.Count == 1)
                    {
                        sSearch = " and (" + scConfig.search[0].field + " like " + sqlStrLike(scConfig.search[0].value) + " ) ";
                    }
                    else if (scols != null)
                    {
                        string ss = "";
                        foreach (string scol in scols)
                        {
                            if (ss != "") ss += " or ";
                            ss = ss + scol + " like " + sqlStrLike(scConfig.search[0].value);
                        }
                        if (ss != "")
                        {
                            sSearch = " and ( " + ss + " ) ";
                        }
                    }
                }

                sOffset = scConfig.offset.ToString();
                sLimit = scConfig.limit.ToString();
            }

            if (sSorter == "") sSorter = defsort;
            sSearch += " " + extrafilter + " ";

            string sql = "SELECT " + idcol + " as recid, * FROM " + table +
                " WHERE 1=1 " + sSearch + " ORDER BY " + sSorter +
                (sOffset != "" ? " OFFSET " + sOffset + " ROWS FETCH NEXT " + sLimit + " ROWS ONLY" : "");
            return ListData0(sql);
        }

        protected ContentResult ListData0(string sql)
        {
            DataTable tbl = Koneksi.GetDataTable(sql);


            string stepId = string.Empty;
            string stepName = string.Empty;

            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(tbl);

            return new ContentResult
            {
                Content = JSONresult,
                ContentType = "application/json"
            };
        }

        public void writeLog(string msg)
        {
            try
            {
                StreamWriter w = new StreamWriter(Server.MapPath("~/Logs/log_" + DateTime.Today.ToString("yyyyMMdd") + ".log"), true);
                w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                w.WriteLine(msg);
                w.WriteLine();
                w.Close();
            }
            catch
            {
            }
        }
        public void writeLog(Exception ex)
        {
            try
            {
                string err = ex.Message;
                try
                {
                    Exception ei = ex.InnerException;
                    int n = 1;
                    while (ei != null && n < 100)
                    {
                        err += "\r\n" + n.ToString() + "> " + ei.Message;
                        ei = ei.InnerException;
                        n++;
                    }
                    if (ex is DbEntityValidationException)
                    {
                        DbEntityValidationException ex2 = (DbEntityValidationException)ex;
                        foreach (var e in ex2.EntityValidationErrors)
                        {
                            err += "\r\n" + e.Entry.Entity.ToString();
                            foreach (var e2 in e.ValidationErrors)
                            {
                                err += "\r\n" + e2.ErrorMessage;
                            }
                        }
                    }
                    err += (ex.StackTrace != null) ? "\r\n" + ex.StackTrace.ToString() : "";
                }
                catch
                {
                    err += " #ERR";
                }
                writeLog(err);
            }
            catch
            {
            }
        }

        protected ActionResult GetFile(string source, string key1, string key2, int key3, int key4, int revision,
            int dl, string cntype, string fn)
        {
            try
            {
                ServiceDocSoapClient ws = new ServiceDocSoapClient();
                byte[] data = ws.GetFileData(source, key1, key2, key3, key4, revision);
                if (data != null)
                {
                    MemoryStream str = new MemoryStream(data);
                    if (dl == 1)
                    {
                        return File(str, cntype, fn);
                    }
                    else
                    {
                        return File(str, cntype);
                    }
                }
            }
            catch (Exception ex)
            {
                writeLog(ex);
            }
            return FileNotFounds();
        }

        protected ActionResult FileNotFounds()
        {
            var fp = Server.MapPath("~/Template/doc_not_found.jpg");
            var fileStream = new FileStream(fp, FileMode.Open, FileAccess.Read);
            return File(fileStream, MimeMapping.GetMimeMapping(fp));
        }

    }
}