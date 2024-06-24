using SimplifikasiFID.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using SimplifikasiFID.ServiceSSO;
using System.Text;
using SimplifikasiFID.ServiceDOC;
using System.IO;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace SimplifikasiFID.Controllers
{
    public class PMSController : Controller
    {
        public SimplyFIDEntities _db = new SimplyFIDEntities();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getalldoc")]
        public async Task<ActionResult> getAllDoc(string jenis_investasi, DateTime start_date, DateTime end_date)
        {
            ResponseFID response = new ResponseFID();
            try
            {

                //check credential
                if (isValid())
                {
                    if (start_date != null || end_date != null)
                    {
                        var enddates = end_date.AddDays(1);
                        var query = _db.V_FID.AsNoTracking()
                        .Where(x => x.sts == 11 && x.CreatedDate >= start_date && x.CreatedDate <= enddates && (jenis_investasi == "" || x.Jenisfid == jenis_investasi))
                        .AsEnumerable()
                        .Select(x => new datas
                        {
                            NoFID = x.NoFID,
                            Judulfid = x.Judulfid,
                            FungsiPengusul = x.Fungsipengusul,
                            JenisInvestasi = x.Jenisfid,
                            NilaiABI = x.NilaiABI,
                            TahunAnggaran = x.TahunAnggaran,
                            CreatedDate = x.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        });

                        response.StatusCode = 200;
                        response.Status = "OK";
                        response.Message = "Success";
                        response.Data = query.ToList();
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.Status = "UNEXPECTED_ERROR";
                        response.Message = "INVALID DATETIME";
                        response.Data = null;
                    }
                }
                else
                {
                    response.StatusCode = 401;
                    response.Status = "Unauthorized";
                    response.Data = null;


                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Status = "UNEXPECTED_ERROR";
                response.Message = ex.ToString();
                response.Data = null;

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> getByDocno(string docno)
        {
            ResponseFID2 response = new ResponseFID2();
            List<dataFiles> lsFiles = new List<dataFiles>();
            try
            {
                if (docno != "")
                {
                    if (isValid())
                    {
                        var query = await _db.V_FID.AsNoTracking()
                        .Where(x => x.NoFID == docno && x.sts == 11)
                         .Select(x => new datas
                         {
                             NoFID = x.NoFID,
                             Judulfid = x.Judulfid,
                             FungsiPengusul = x.Fungsipengusul,
                             JenisInvestasi = x.Jenisfid,
                             NilaiABI = x.NilaiABI,
                             TahunAnggaran = x.TahunAnggaran
                         })
                        .FirstOrDefaultAsync();


                        if (query != null)
                        {
                            //get files
                            var files = await _db.M_UploadFiles.Where(x => x.NoFID == query.NoFID)
                                .ToListAsync();


                            foreach (var upl in files)
                            {
                                dataFiles dataFiles = new dataFiles();
                                int dl = 1;
                                string nofid_ = upl.NoFID.Replace("/", "_");
                                if (upl != null)
                                {
                                    var ssfl = GetFile("E-FID", "FID_FILE", nofid_, upl.UploadID, int.Parse(upl.Key4), 0, dl, upl.ContentType, upl.FileName);

                                    dataFiles.ContentType = upl.ContentType;
                                    dataFiles.FileName = upl.FileName;
                                    dataFiles.Files = ssfl;

                                    lsFiles.Add(dataFiles);
                                }
                            }


                            response.StatusCode = 200;
                            response.Status = "OK";
                            response.Data = query;
                            response.files = lsFiles;

                            var result = Json(response, JsonRequestBehavior.AllowGet);
                            result.MaxJsonLength = int.MaxValue;

                            return result;
                        }
                        else
                        {
                            response.StatusCode = 200;
                            response.Status = "OK";
                            response.Data = null;
                            response.Message = "Document Not Found";
                            response.files = lsFiles;

                            var result = Json(response, JsonRequestBehavior.AllowGet);
                            result.MaxJsonLength = int.MaxValue;

                            return result;
                        }

                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.Status = "UNEXPECTED_ERROR";
                        response.Message = "Please Insert Document Number";
                        response.Data = null;

                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    response.StatusCode = 401;
                    response.Status = "Unauthorized";
                    response.Data = null;

                    return Json(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Status = "UNEXPECTED_ERROR";
                response.Message = ex.ToString();
                response.Data = null;

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        private bool isValid()
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            try
            {
                string authHeader = this.HttpContext.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    var username = usernamePassword.Substring(0, seperatorIndex);
                    var password = usernamePassword.Substring(seperatorIndex + 1);

                    if (ws.ValidateUser(username, password))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("The authorization header is either empty or isn't Basic.");
                }


                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.ToString());
            }
        }

        protected byte[] GetFile(string source, string key1, string key2, int key3, int key4, int revision,
            int dl, string cntype, string fn)
        {
            try
            {
                ServiceDocSoapClient ws = new ServiceDocSoapClient();
                byte[] data = ws.GetFileData(source, key1, key2, key3, key4, revision);
                if (data != null)
                {
                    return data;
                }

                return null;

            }
            catch (Exception ex)
            {
                writeLog(ex);
                return null;
            }
        }

        protected ActionResult FileNotFounds()
        {
            var fp = Server.MapPath("~/Template/doc_not_found.jpg");
            var fileStream = new FileStream(fp, FileMode.Open, FileAccess.Read);
            return File(fileStream, MimeMapping.GetMimeMapping(fp));
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
    }
}