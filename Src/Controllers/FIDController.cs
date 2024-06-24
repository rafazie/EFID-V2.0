using SimplifikasiFID.Models;
using SimplifikasiFID.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using SimplifikasiFID.ServiceDOC;
using System.Configuration;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SimplifikasiFID.Services;
using Rotativa;

namespace SimplifikasiFID.Controllers
{
    public class FIDController : BaseController
    {
        private readonly SimplyFIDEntities _context = new SimplyFIDEntities();
        private readonly FIDServices _services;

        public FIDController()
        {
            _services = new FIDServices(_context);
        }

        [Authorize]
        public ActionResult vFID(string category)
        {
            return View((object)category);
        }

        [Authorize]
        public ActionResult ListFID(string category)
        {
            var fungsi = db.M_Fungsi.ToList();
            ViewBag.fungsi = fungsi;

            return View((object)category);
        }

        [Authorize]
        public ActionResult NotFound()
        {
            return View();
        }

        [Authorize]
        public ContentResult ListFungsi(string request)
        {
            return ListData(request, "FungsiName asc",
                    new string[] { "ID", "FungsiName" }, "ID", "M_Fungsi");
        }

        [Authorize]
        public ContentResult GetList(string sts, string year, string request, string id, string category)
        {
            try
            {
                ViewBag.sts = sts;
                string Jenis = "";
                string sSearch = "";
                if (id != null)
                {
                    sSearch = " AND DocID = " + sqlStr(id);
                }

                if (sts == "90")
                {
                    sSearch += category == "BD"
                        ? " and (sts = " + sqlStr("5") + " AND Jenisfid = " + sqlStr("BD") + ") "
                        : " and (sts = " + sqlStr("8") + " AND Jenisfid IN (" + sqlStr("NON-BD") + ", " + sqlStr("NON BD") + ")) ";
                }

                if (sts == "91")
                {
                    sSearch += category == "BD"
                       ? " and (sts = " + sqlStr("6") + " AND Jenisfid = " + sqlStr("BD") + ") "
                       : " and (sts = " + sqlStr("9") + " AND Jenisfid IN (" + sqlStr("NON-BD") + ", " + sqlStr("NON BD") + ")) ";
                }

                if (sts == "92")
                {
                    sSearch += category == "BD"
                       ? " and (sts = " + sqlStr("8") + " AND Jenisfid = " + sqlStr("BD") + ") "
                       : " and (sts = " + sqlStr("5") + " AND Jenisfid IN (" + sqlStr("NON-BD") + ", " + sqlStr("NON BD") + ")) ";
                }

                if (sts == "2")
                {
                    sSearch += " and sts = " + sqlStr(sts) + " Or sts = '12'";
                }

                if (sts != null && sts != "" && sts != "90" && sts != "91" && sts != "92" && sts != "2")
                {
                    sSearch += " and sts = " + sqlStr(sts);
                }

                if (year != null && year != "")
                {
                    sSearch += " and TahunAnggaran = " + sqlStr(year);
                }

                if (category != null && category != "")
                {
                    sSearch += " AND Jenisfid = " + sqlStr(category);
                }
                SimplyFIDEntities dbs = new SimplyFIDEntities();

                var role = dbs.M_User.Where(x => x.username.Equals(User.Identity.Name)).Select(x => x.flag).FirstOrDefault();
                if (role == 1)
                {
                    sSearch += " and CreatedBy= " + sqlStr(User.Identity.Name.ToString());
                }

                return ListData(request, "DocID asc",
                    new string[] { "DocID", "JudulFID" }, "DocID", "V_FID", sSearch);
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.ToString());
            }
        }

        [Authorize]
        public ContentResult GetDocument(string NoFID)
        {
            try
            {
                //return ListData0("select UploadID recid, 'Lampiran-E-FID' as DocType, * from M_UploadFiles a where a.NoFID = " + sqlStr(NoFID));
                string sql = "SELECT UploadID recid, 'Lampiran-E-FID' as DocType, * FROM( SELECT TOP 1 * FROM M_UploadFiles WHERE Category = 'PS' AND NoFID = '" + NoFID + "' " +
                                  "ORDER BY UploadDate DESC " +
                                  "UNION " +
                                  "SELECT TOP 1 * FROM M_UploadFiles WHERE Category = 'CBA' AND NoFID = '" + NoFID + "' ORDER BY UploadDate DESC " +
                                  "UNION " +
                                  "SELECT TOP 1 * FROM M_UploadFiles WHERE Category = 'HR' AND NoFID = '" + NoFID + "' ORDER BY UploadDate DESC " +
                                  "UNION " +
                                  "SELECT TOP 1 * FROM M_UploadFiles WHERE Category = 'FID' AND NoFID = '" + NoFID + "' ORDER BY UploadDate DESC " +
                                  "UNION " +
                                  "SELECT * FROM M_UploadFiles WHERE Category IS NULL AND NoFID = '" + NoFID + "')" +
                                  " AS CombinedResults ORDER BY UploadID DESC";

                return ListData0(sql);

            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.Message.ToString());
            }
        }

        [Authorize]
        public ActionResult GetDocumentFiles(int id)
        {
            ServiceDocSoapClient docman = new ServiceDocSoapClient();
            try
            {

                M_UploadFiles upl = db.M_UploadFiles.FirstOrDefault(t => t.UploadID.Equals(id));
                int dl = 1;
                string nofid_ = upl.NoFID.Replace("/", "_");
                if (upl != null)
                {
                    //return GetFile("E-FID", "FID_FILE", nofid_, upl.UploadID, int.Parse(upl.Key4), 0, dl, upl.ContentType, upl.FileName);
                    var data = docman.GetFileDataById(int.Parse(upl.Key4), "testing.fid1", "user@default");

                    var fileResult = Convert.ToBase64String(data);

                    //return Json(data, JsonRequestBehavior.AllowGet);
                    MemoryStream ms = new MemoryStream(data);
                    return File(ms, "application/pdf");
                }

                return FileNotFounds();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [Authorize]
        public ContentResult DeleteDoc(int id)
        {
            try
            {
                string nofid = "";

                using (var trans = db.Database.BeginTransaction())
                {

                    T_FID fid = db.T_FID.FirstOrDefault(t => t.DocID.Equals(id));
                    nofid = fid.NoFID;

                    db.T_FID.Remove(fid);
                    db.SaveChanges();

                    var upl = db.M_UploadFiles.Where(x => x.NoFID.Equals(nofid)).ToList();
                    db.M_UploadFiles.RemoveRange(upl);
                    db.SaveChanges();

                    var catatan = db.T_Catatan.Where(x => x.NoFID.Equals(nofid)).ToList();
                    db.T_Catatan.RemoveRange(catatan);
                    db.SaveChanges();

                    var email = db.M_Email.Where(x => x.NoFID.Equals(nofid)).ToList();
                    db.M_Email.RemoveRange(email);
                    db.SaveChanges(); ;

                    trans.Commit();
                    return JsonContent(new { Status = 1, Message = "Berhasil!" });
                }

            }
            catch (Exception ex)
            {
                writeLog(ex);
                throw new Exception(ex.ToString());
            }
        }

        [Authorize]
        public JsonResult DeleteFiles(int id)
        {
            try
            {
                M_UploadFiles upl = db.M_UploadFiles.FirstOrDefault(t => t.UploadID.Equals(id));
                string nofid_ = upl.NoFID.Replace("/", "_");

                ServiceDocSoapClient ws = new ServiceDocSoapClient();
                //ws.DeleteFile("E-FID", "FID_FILE", nofid_, upl.UploadID, int.Parse(upl.Key4), 0, User.Identity.Name);
                ws.DeleteFileById(Convert.ToInt32(upl.Key4), "testing.fid1", "user@default", User.Identity.Name);

                db.M_UploadFiles.Remove(upl);
                db.SaveChanges();

                return Json(new { Result = "Success" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }


        [Authorize]
        [ValidateInput(false)]
        public async Task<JsonResult> SaveFIDAsync(FormCollection form)
        {
            try
            {
                //var filePS = new T_FIDFiles
                //{
                //    NoFID = "FID/BPD/BD/2024/000015",
                //    Category = "PS",
                //    HtmlTag = form["filePS"],
                //    CreatedAt = DateTime.Now,
                //    CreatedBy = User.Identity.Name
                //};
                //string encodedNoFID = HttpUtility.UrlEncode(filePS.NoFID);
                //var actionPdf = new ActionAsPdf("OnlineForm/TesForm?nofid=" + encodedNoFID)
                //{
                //    // Adjust PDF settings as needed
                //    FileName = "File_" + filePS.Category + ".pdf",
                //    PageSize = Rotativa.Options.Size.A4,
                //    PageOrientation = Rotativa.Options.Orientation.Portrait,
                //    PageMargins = new Rotativa.Options.Margins(1, 1, 1, 1)
                //};

                //byte[] applicationPDFData = actionPdf.BuildPdf(ControllerContext);
                //var strFile = Convert.ToBase64String(applicationPDFData);

                var query = await _services.SaveFID(form, User.Identity.Name, Request, ControllerContext);
                if (query.sts)
                {
                    return Json(new { Result = "success" });
                }
                else
                {
                    return Json(new { Result = "failed" });
                }
            }
            catch (Exception ex)
            {
                writeLog(ex);
                return Json(new { Result = "Error", Msg = ex.Message });
            }

        }

        [Authorize]
        public ContentResult ListCatatan(string NoFID)
        {
            return ListData0("select CatatanID recid, * from T_Catatan where NoFID = " + sqlStr(NoFID));
        }

        private string DocID(string input)
        {
            try
            {
                string docNo;
                SimplyFIDEntities context = new SimplyFIDEntities();

                var query = context.T_FID.OrderByDescending(x => x.NoFID)
                    .FirstOrDefault(x => x.Jenisfid.Equals(input))?.NoFID;


                if (query == null)
                {
                    docNo = "FID/BPD/" + input.Replace(" ", "") + "/" + DateTime.Now.Year + "/" + "000001";


                }
                else
                {
                    int sub = int.Parse(query.Substring(query.Length - 6)) + 1;

                    docNo = "FID/BPD/" + input.Replace(" ", "") + "/" + DateTime.Now.Year + "/" + sub.ToString().PadLeft(6, '0');

                }

                return docNo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [Authorize]
        public ContentResult GetEmailList(string NoFID)
        {
            return ListData0("select ID recid, * from M_Email where NoFID=" + sqlStr(NoFID));
        }

        //===========start
        [Authorize]
        public JsonResult SaveEmail(FormCollection form)
        {
            try
            {
                string noFid = form["NoFID"];
                string jabatan = form["Jabatan"];
                using (var trans = db.Database.BeginTransaction())
                {
                    if (form["stsEmail"] == "1")
                    {
                        var query = db.M_EmailManager.Where(x => x.Jabatan.Equals(jabatan)).FirstOrDefault();
                        query.Email = form["Email"];

                        db.SaveChanges();
                    }

                    M_Email email = new M_Email();
                    email.NoFID = noFid;
                    email.Email = form["Email"];
                    email.Jabatan = form["Jabatan"];
                    email.Status = form["Status"];

                    db.M_Email.Add(email);
                    db.SaveChanges();

                    trans.Commit();
                    return Json(new { Result = "Success" });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        //===========end

        [Authorize]
        public JsonResult DeleteEmail(int id)
        {
            try
            {
                M_Email email = db.M_Email.FirstOrDefault(t => t.ID.Equals(id));

                if (email != null)
                {
                    db.M_Email.Remove(email);
                    db.SaveChanges();

                    return Json(new { Status = 1, Message = "Berhasil!" });
                }

                return Json(new { Status = 1, Message = "Berhasil!" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        //==========start
        [Authorize]
        public JsonResult CekEmail(FormCollection form)
        {
            try
            {
                string NoFID = form["nofid"];
                string jenis = form["jenis"];

                var query = db.M_Email.Where(x => x.NoFID.Equals(NoFID) && x.Status.Equals("emailTo")).Count();
                var query2 = db.M_Email.Where(x => x.NoFID.Equals(NoFID) && x.Status.Equals("emailTo")).Select(x => x.Jabatan).ToList();

                List<string> emailMan = new List<string>();
                emailMan.Add("ManBPD");
                emailMan.Add("ManFBS");
                emailMan.Add("ManCNL");
                emailMan.Add("ManCSPRM");

                foreach (string email in emailMan)
                {
                    string hasil = query2.Find(x => x.Contains(email));

                    if (hasil != null)
                    {
                        continue;
                    }
                    else
                    {
                        string result = "";
                        if (email == "ManCNL")
                        {
                            result = "Man. Corsec & Legal";
                        }
                        else
                        {
                            result = email.Insert(3, ". ");
                        }


                        return Json(new { Result = "Error", Msg = "Email " + result + " Belum Lengkap" });
                    }

                }

                return Json(new { Result = "Succes" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Msg = ex.Message });
            }
        }

        public JsonResult GetEmailMan(string man)
        {
            try
            {
                var query = db.M_EmailManager.Where(x => x.Jabatan.Equals(man))
                    .Select(x => x.Email).FirstOrDefault();

                return Json(query);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Msg = ex.Message });
            }
        }
        // ================== end


        private object decodeImager(FormCollection form)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();

                var filepd = form["filePS"];
                var fileCBA = form["fileCBA"];

                doc.LoadHtml(filepd);

                HtmlNode imgPS = doc.DocumentNode.SelectSingleNode("//img[@id='img-tw']");

                if (imgPS != null)
                {
                    string src = imgPS.GetAttributeValue("src", "");

                    var hasil = src.Split(',')[1];

                    byte[] array1 = Convert.FromBase64String(hasil);

                }


                byte[] array2 = Convert.FromBase64String(fileCBA);



                return new { array2 };
            }
            catch (Exception ez)
            {
                throw new Exception(ez.ToString());
            }
        }
    }
}
