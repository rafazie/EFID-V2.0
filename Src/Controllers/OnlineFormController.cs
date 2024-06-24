using Newtonsoft.Json;
using Rotativa;
using SimplifikasiFID.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using SimplifikasiFID.Services;

namespace SimplifikasiFID.Controllers
{
    [Authorize]
    public class OnlineFormController : BaseController
    {
        private readonly SimplyFIDEntities _context = new SimplyFIDEntities();
        private readonly FIDServices _services;

        public OnlineFormController()
        {
            _services = new FIDServices(_context);
        }

        [Authorize]
        [ValidateInput(false)]
        [Obsolete]
        public async Task<JsonResult> SaveFID(FormCollection form)
        {

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

        public async Task<ActionResult> PengajuanUsulan(string category)
        {
            var pengajuanUsulan = new PengajuanUsulanDTO
            {
                Category = category,
                //Email = await GetUserEmailAsync(User.Identity.Name),
                Email = "email@email.com",
                M_Fungsi = db.M_Fungsi.ToList()
            };

            return View(pengajuanUsulan);
        }

        public async Task<ActionResult> PengajuanFID(string nofid)
        {
            var data = await _context.T_FID.Where(x => x.NoFID == nofid).FirstOrDefaultAsync();

            await _services.UpdateEmailManagerAsync(nofid);

            return View(data);
        }

        public async Task<ActionResult> Editform(string nofid)
        {
            var query = new EditFormDTO
            {
                T_FID = await db.T_FID.Where(x => x.NoFID == nofid).FirstOrDefaultAsync(),
                M_UploadFiles = await db.M_UploadFiles.Where(x => x.NoFID == nofid).ToListAsync()
            };

            return View(query);
        }

        public async Task<ActionResult> Approveform(string nofid)
        {
            var query = new EditFormDTO
            {
                T_FID = await db.T_FID.Where(x => x.NoFID == nofid).FirstOrDefaultAsync(),
                M_UploadFiles = await db.M_UploadFiles.Where(x => x.NoFID == nofid).ToListAsync()
            };

            return View(query);
        }

        public ActionResult ProjectSummary(string category)
        {
            if (category == "bd")
            {
                return RedirectToAction("ProjectSummaryBD", "FID");
            }
            else
            {
                return RedirectToAction("ProjectSummaryNBD", "FID");
            }
        }

        public ActionResult ProjectSummaryBD()
        {
            //return new ViewAsPdf()
            //{
            //    PageOrientation = Rotativa.Options.Orientation.Portrait,
            //    FileName = "File pdf PS",
            //    PageSize = Rotativa.Options.Size.A4,
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        public ActionResult ProjectSummaryNBD()
        {
            return View();
        }

        public ActionResult FID_BD()
        {
            //return new ViewAsPdf()
            //{
            //    FileName = "File pdf FID",
            //    PageSize = Rotativa.Options.Size.A4,
            //    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        public ActionResult FID_NBD()
        {
            //return new ViewAsPdf()
            //{
            //    FileName = "File pdf FID",
            //    PageSize = Rotativa.Options.Size.A4,
            //    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        public ActionResult HR_BD()
        {
            //return new ViewAsPdf()
            //{
            //    FileName = "File pdf HR",
            //    PageSize = Rotativa.Options.Size.A4,
            //    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        public ActionResult HR_NBD()
        {
            //return new ViewAsPdf()
            //{
            //    FileName = "File pdf HR",
            //    PageSize = Rotativa.Options.Size.A4,
            //    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        public ActionResult FormCBA()
        {
            //return new ViewAsPdf()
            //{
            //    PageOrientation = Rotativa.Options.Orientation.Landscape,
            //    FileName = "File pdf CBA",
            //    PageSize = Rotativa.Options.Size.A4,
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View();
        }

        [ValidateInput(false)]
        public ActionResult FormPDF(string nofid, string category)
        {
            var data = db.M_UploadFiles.Where(x => x.NoFID == nofid && x.Category == category)
                .OrderByDescending(x => x.UploadDate).FirstOrDefault();

            return View((object)data.HtmlTag);
        }

        [ValidateInput(false)]
        public async Task<JsonResult> SpecimenDoc(string nofid)
        {
            try
            {
                var topHR = await _context.M_UploadFiles.Where(f => f.Category == "HR" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                var topFID = await _context.M_UploadFiles.Where(f => f.Category == "FID" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                //var dataFiles = await _context.T_FIDFiles.Where(x => x.NoFID == nofid).ToListAsync();
                var data = topHR.Union(topFID)
                    .OrderByDescending(f => f.UploadDate)
                    .Select(x => new M_UploadFiles
                    {
                        NoFID = x.NoFID,
                        Category = x.Category,
                        HtmlTag = x.HtmlTag,
                        UploadDate = x.UploadDate,
                        UploadBy = x.UploadBy
                    });
                var signature = await AddSignature();

                var result = new
                {
                    data = data,
                    signature = signature
                };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error = " + ex.Message });
            }
        }

        public async Task<ActionResult> TesForm2()
        {
            var data = db.M_UploadFiles.Where(x => x.UploadID == 396).FirstOrDefault();

            //return new ViewAsPdf(data)
            //{
            //    FileName = "File_" + data.Category + ".pdf",
            //    PageSize = Rotativa.Options.Size.A4,
            //    PageOrientation = Rotativa.Options.Orientation.Portrait,
            //    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
            //    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            //};
            return View(data);
        }

        public async Task<ActionResult> TesForm3()
        {
            //var data = db.M_UploadFiles.Where(x => x.UploadID == 269).FirstOrDefault();

            return new ViewAsPdf()
            {
                FileName = "File_Testing.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
            };
            //return View();
        }

        [ValidateInput(false)]
        public JsonResult addOnlineForm(FormCollection form)
        {
            try
            {
                var htmltag = form["htmlContent"];
                var caegory = form["category"];

                var data = db.M_TemplateForm.Where(x => x.Category == caegory).FirstOrDefault();

                using (var trans = db.Database.BeginTransaction())
                {
                    if (data == null)
                    {
                        var datas = new M_TemplateForm
                        {
                            Category = caegory,
                            HtmlTag = htmltag,
                            CreateAt = DateTime.Now,
                            CreateBy = User.Identity.Name
                        };

                        db.M_TemplateForm.Add(datas);
                    }
                    else
                    {
                        data.Category = caegory;
                        data.HtmlTag = htmltag;
                        data.CreateAt = DateTime.Now;
                        data.CreateBy = User.Identity.Name;
                    }

                    db.SaveChanges();
                    trans.Commit();

                    return Json(new { Result = data.Category });
                }
            }
            catch (Exception ex)
            {
                writeLog(ex);
                return Json(new { Result = "Error", Msg = ex.Message });
            }
        }

        public JsonResult getTemplateForm(string category)
        {
            try
            {
                var data = db.M_TemplateForm.Where(x => x.Category == category).FirstOrDefault();

                return Json(data.HtmlTag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                writeLog(ex);
                return Json(new { Result = "Error", Msg = ex.Message });
            }
        }

        private async Task<string> GetUserEmailAsync(string userId)
        {
            try
            {
                var data = await _services.GetListUserByKdOrg();

                var fungsi = data
                    .OrderBy(x => x.Fungsi)
                    .Select(x => x.Fungsi)
                                    .Distinct()
                    .ToList();

                var ict = data.Where(x => x.Fungsi == "ICT").ToList();
                var json = JsonConvert.SerializeObject(ict);

                using (var client = new HttpClient())
                {
                    var url = "http://ptprkpapp01:8031/User/GetUserDetail?username=" + userId + "&NIP=";

                    client.BaseAddress = new Uri(url);
                    var response = await client.GetAsync(url);
                    var rawString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<UserDTO>(rawString);

                    if (result.Data == null)
                    {
                        return "email@email.com";
                    }
                    else
                    {
                        return result.Data.Email;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error = " + ex.Message);
            }
        }

        public async Task<ActionResult> SampleForm()
        {
            return View();
        }

        public async Task<JsonResult> AddSignature()
        {
            try
            {
                // Add Manager BPD Here
                string ManName = "Seno Wibowo";
                string imgpath = "E:\\Pertamina Retail\\Fauzi _sourceCode _ 2021.06.22\\Simpifikasi FID\\Update Customer Day 2024\\img\\speciment.JPG";

        
                var imgByte = System.IO.File.ReadAllBytes(imgpath);
                string img64string = Convert.ToBase64String(imgByte);

                if (!string.IsNullOrEmpty(img64string))
                {
                    var data = new 
                    {
                        username = ManName,
                        img64string = img64string
                    };

                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var data = new
                    {
                        username = ManName,
                        img64string = ""
                    };

                    return Json(data, JsonRequestBehavior.AllowGet);
                }             

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}