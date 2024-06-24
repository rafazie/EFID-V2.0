using Rotativa;
using SimplifikasiFID.Classes;
using SimplifikasiFID.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rotativa.Options; 
using System.IO;
using SimplifikasiFID.ServiceDOC;
using SimplifikasiFID.Controllers;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Mail;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Data.Entity;

namespace SimplifikasiFID.Services
{
    public class FIDServices
    {
        private readonly SimplyFIDEntities _context;
        private readonly string _wkhtmltopdfPath;
        bool _gen;

        public FIDServices(SimplyFIDEntities context)
        {
            var virtualPath = ConfigurationManager.AppSettings["WkhtmltopdfPath"];
            _wkhtmltopdfPath = HttpContext.Current.Server.MapPath(virtualPath);
            _context = context;
            _gen = false;
        }

        public string DocID(string input)
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

        [Obsolete]
        public async Task<(bool sts, string data, string msg)> SaveFID(FormCollection form, string username, HttpRequestBase request, ControllerContext context)
        {
            try
            {
                T_FID fid = new T_FID();
                T_Catatan catatan = new T_Catatan();

                string docNo = DocID(form["JenisFID"]);

                string flag = form["Flag_update"];
                string NoFID = form["NoFID"];

                using (var _trans = _context.Database.BeginTransaction())
                {
                    if (flag == "DRAFT")
                    {

                        string abi = form["NilaiABI"];

                        fid.NoFID = docNo;
                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.CreatedBy = username;
                        fid.CreatedDate = DateTime.Now;
                        fid.Status = 0;

                        _context.T_FID.Add(fid);
                        _context.SaveChanges();

                        if (!form["Email"].Contains("@"))
                        {
                            throw new Exception("error email");
                        }

                        M_Email email = new M_Email();
                        email.NoFID = docNo;
                        email.Username = form["UserName"];
                        email.Email = form["Email"];
                        email.Jabatan = form["Jabatan"];

                        _context.M_Email.Add(email);
                        _context.SaveChanges();

                        var filePS = new M_UploadFiles
                        {
                            NoFID = docNo,
                            ContentType = ".pdf",
                            Category = "PS",
                            FileName = "File_PS",
                            HtmlTag = form["filePS"],
                            UploadBy = username,
                            UploadDate = DateTime.Now,
                            Status = 1,
                            StatusSign = false
                        };
                        _context.M_UploadFiles.Add(filePS);
                        _context.SaveChanges();

                        if (form["fileCBA"] != null)
                        {
                            var fileCBA = new M_UploadFiles
                            {
                                NoFID = docNo,
                                ContentType = ".pdf",
                                Category = "CBA",
                                FileName = "File_CBA",
                                HtmlTag = form["fileCBA"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1,
                                StatusSign = false
                            };
                            _context.M_UploadFiles.Add(fileCBA);
                            _context.SaveChanges();
                        }

                        _gen = true;
                    }
                    if (flag == "NEW")
                    {
                        string abi = form["NilaiABI"];

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 1;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }
                    if (flag == "NEW-EDIT")
                    {
                        string abi = form["NilaiABI"];

                        fid = await _context.T_FID.FirstOrDefaultAsync(x => x.NoFID.Equals(NoFID));
                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 1;

                        _context.SaveChanges();

                        if (form["filePS"] != null)
                        {
                            var filePS = new M_UploadFiles
                            {
                                NoFID = fid.NoFID,
                                ContentType = ".pdf",
                                Category = "PS",
                                FileName = "File_PS",
                                HtmlTag = form["filePS"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1,
                                StatusSign = false
                            };
                            _context.M_UploadFiles.Add(filePS);
                            _context.SaveChanges();
                        }

                        if (form["fileCBA"] != null)
                        {
                            var fileCBA = new M_UploadFiles
                            {
                                NoFID = docNo,
                                ContentType = ".pdf",
                                Category = "CBA",
                                FileName = "File_CBA",
                                HtmlTag = form["fileCBA"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1,
                                StatusSign = false
                            };
                            _context.M_UploadFiles.Add(fileCBA);
                            _context.SaveChanges();
                        }

                        _gen = true;

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }
                    if (flag == "UPREV")
                    {
                        string abi = form["NilaiABI"];

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));

                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 1;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }

                    if (flag == "UPREV2")
                    {
                        string abi = form["NilaiABI"];

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));

                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 2;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }
                    if (flag == "SIG1" || flag == "SIG2" || flag == "SIG3" || flag == "SIG4")
                    {
                        var status = setStatusSign(flag);
                        string abi = form["NilaiABI"];

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.Judulfid = form["JudulFID"];
                        fid.TahunAnggaran = form["ThnAng"];
                        fid.NilaiABI = long.Parse(abi);
                        fid.Jenisfid = form["JenisFID"];
                        fid.Fungsipengusul = form["FungsiPengusul"];
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = status;

                        _context.SaveChanges();

                        if (form["filePS"] != null)
                        {
                            var filePS = new M_UploadFiles
                            {
                                NoFID = fid.NoFID,
                                ContentType = ".pdf",
                                Category = "PS",
                                FileName = "File_PS",
                                HtmlTag = form["filePS"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1
                            };
                            _context.M_UploadFiles.Add(filePS);
                            _context.SaveChanges();
                        }

                        if (form["fileCBA"] != null)
                        {
                            var fileCBA = new M_UploadFiles
                            {
                                NoFID = fid.NoFID,
                                ContentType = ".pdf",
                                Category = "CBA",
                                FileName = "File_CBA",
                                HtmlTag = form["fileCBA"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1
                            };
                            _context.M_UploadFiles.Add(fileCBA);
                            _context.SaveChanges();
                        }

                        _gen = true;

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }
                    if (flag == "APPROVE")
                    {
                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 3;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);

                    }
                    if (flag == "REVISI")
                    {

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 4;

                        _context.SaveChanges();

                        catatan.NoFID = NoFID;
                        catatan.Catatan = form["Catatan"];
                        catatan.CreatedDate = DateTime.Now;
                        catatan.CreatedUser = username;

                        _context.T_Catatan.Add(catatan);
                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }

                    if (flag == "REVISI2")
                    {

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 14;

                        _context.SaveChanges();

                        catatan.NoFID = NoFID;
                        catatan.Catatan = form["Catatan"];
                        catatan.CreatedDate = DateTime.Now;
                        catatan.CreatedUser = username;

                        _context.T_Catatan.Add(catatan);
                        _context.SaveChanges();

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }
                    if (flag == "DRAFTFID")
                    {

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 13;

                        _context.SaveChanges();

                        catatan.NoFID = NoFID;
                        catatan.Catatan = form["Catatan"];
                        catatan.CreatedDate = DateTime.Now;
                        catatan.CreatedUser = username;

                        _context.T_Catatan.Add(catatan);
                        _context.SaveChanges();
                    }
                    if (flag == "FID")
                    {
                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 5;

                        _context.SaveChanges();

                        if (form["fileHR"] != null)
                        {
                            var fileHR = new M_UploadFiles
                            {
                                NoFID = fid.NoFID,
                                ContentType = ".pdf",
                                Category = "HR",
                                FileName = "File_HR",
                                HtmlTag = form["fileHR"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1
                            };
                            _context.M_UploadFiles.Add(fileHR);
                            _context.SaveChanges();
                        }

                        if (form["fileFID"] != null)
                        {
                            var fileFID = new M_UploadFiles
                            {
                                NoFID = fid.NoFID,
                                ContentType = ".pdf",
                                Category = "FID",
                                FileName = "File_FID",
                                HtmlTag = form["fileFID"],
                                UploadBy = username,
                                UploadDate = DateTime.Now,
                                Status = 1
                            };
                            _context.M_UploadFiles.Add(fileFID);
                            _context.SaveChanges();
                        }

                        _gen = true;

                        SendEmailAsync(NoFID, flag, form["JenisFID"]);
                    }

                    if (flag.StartsWith("MAN"))
                    {
                        fid = _context.T_FID.Where(x => x.NoFID.Equals(NoFID)).FirstOrDefault();

                        if (fid.Jenisfid == "BD")
                        {
                            if (flag == "MANBPD") // apprv. Man. BPD
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 6;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                if (form["fileFID"] != null)
                                {
                                    var fileFID = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "FID",
                                        FileName = "File_FID",
                                        HtmlTag = form["fileFID"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileFID);
                                    _context.SaveChanges();
                                }

                                _gen = true;

                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANFBS") // apprv. Man. FBS
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 7;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                if (form["fileFID"] != null)
                                {
                                    var fileFID = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "FID",
                                        FileName = "File_FID",
                                        HtmlTag = form["fileFID"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileFID);
                                    _context.SaveChanges();
                                }

                                _gen = true;

                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANCNL") // apprv. Man. Corsec & Legal
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 8;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                if (form["fileFID"] != null)
                                {
                                    var fileFID = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "FID",
                                        FileName = "File_FID",
                                        HtmlTag = form["fileFID"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileFID);
                                    _context.SaveChanges();
                                }

                                _gen = true;

                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANCSPRM") // apprv. Man. CSPRM
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 9;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                if (form["fileHR"] != null)
                                {
                                    var fileHR = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "HR",
                                        FileName = "File_HR",
                                        HtmlTag = form["fileHR"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileHR);
                                    _context.SaveChanges();
                                }

                                if (form["fileFID"] != null)
                                {
                                    var fileFID = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "FID",
                                        FileName = "File_FID",
                                        HtmlTag = form["fileFID"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileFID);
                                    _context.SaveChanges();
                                }

                                _gen = true;

                                SendEmailAsync(NoFID, "SIGNA", form["JenisFID"]);
                            }
                        }
                        else
                        {
                            if (flag == "MANCSPRM") // apprv. Man. CSPRM
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 9;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                if (form["fileFID"] != null)
                                {
                                    var fileFID = new M_UploadFiles
                                    {
                                        NoFID = fid.NoFID,
                                        ContentType = ".pdf",
                                        Category = "FID",
                                        FileName = "File_FID",
                                        HtmlTag = form["fileFID"],
                                        UploadBy = username,
                                        UploadDate = DateTime.Now,
                                        Status = 1
                                    };
                                    _context.M_UploadFiles.Add(fileFID);
                                    _context.SaveChanges();
                                }

                                _gen = true;

                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANFBS") // apprv. Man. FBS
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 7;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANCNL") // apprv. Man. Corsec & Legal
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 8;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();
                                
                                SendEmailAsync(NoFID, flag, form["JenisFID"]);
                            }
                            else if (flag == "MANBPD") // apprv. Man. BPD
                            {
                                fid.UpdateUser = username;
                                fid.UpdateDate = DateTime.Now;
                                fid.Status = 6;
                                _context.SaveChanges();

                                catatan.NoFID = NoFID;
                                catatan.Catatan = form["Catatan"];
                                catatan.CreatedDate = DateTime.Now;
                                catatan.CreatedUser = username;

                                _context.T_Catatan.Add(catatan);
                                _context.SaveChanges();

                                SendEmailAsync(NoFID, "SIGNA", form["JenisFID"]);
                            }
                        }
                    }
                    if (flag == "SIGNP")
                    {

                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 10;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, "SIGNU", form["JenisFID"]);
                    }


                    if (flag == "SIGN")
                    {
                        fid = _context.T_FID.FirstOrDefault(x => x.NoFID.Equals(NoFID));
                        fid.UpdateUser = username;
                        fid.UpdateDate = DateTime.Now;
                        fid.Status = 11;

                        _context.SaveChanges();

                        SendEmailAsync(NoFID, "FINAL", form["JenisFID"]);
                    }

                    if (request.Files.Count > 0)
                    {
                        try
                        {
                            ServiceDocSoapClient docman = new ServiceDocSoapClient();

                            foreach (string file in request.Files)
                            {
                                var fileContent = request.Files[file];

                                if (fileContent != null && fileContent.ContentLength > 0)
                                {
                                    var len = fileContent.ContentLength;
                                    string filename = System.IO.Path.GetFileName(fileContent.FileName);
                                    string filetype = System.IO.Path.GetExtension(fileContent.FileName);
                                    string ContentType = GetMimeType(fileContent.FileName);
                                    string nofid_;
                                    int key4 = int.Parse(DateTime.Now.ToString("ddyy"));

                                    M_UploadFiles upload = new M_UploadFiles();
                                    if (flag == "ADD" || flag == "DRAFT")
                                    {
                                        upload.NoFID = docNo;
                                        nofid_ = docNo;
                                    }
                                    else
                                    {
                                        upload.NoFID = NoFID;
                                        nofid_ = NoFID;
                                    }

                                    nofid_ = nofid_.Replace("/", "_");

                                    #region "send to Docman"
                                    Stream str = fileContent.InputStream;
                                    BinaryReader binReader = new BinaryReader(str);
                                    byte[] data = binReader.ReadBytes((int)str.Length);

                                    string result_ = docman.PutFile3("E-FID", "FID_FILE", nofid_, upload.UploadID, key4, "E-FID", 0,
                                        filename, ContentType, "FID_FILE", username, data, filename);

                                    var resultDocman = JsonConvert.DeserializeObject<resultDocman>(result_);

                                    upload.ContentType = ContentType;
                                    upload.FileName = filename;
                                    upload.Key4 = resultDocman.Result;
                                    upload.UploadBy = username;
                                    upload.UploadDate = DateTime.Now;
                                    upload.Status = 1;

                                    _context.M_UploadFiles.Add(upload);
                                    _context.SaveChanges();
                                    if (result_ != "")
                                    {
                                        //throw new Exception(result_);
                                    }
                                    #endregion

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return (false, null, ex.Message);
                        }
                    }

                    _trans.Commit();

                    if (_gen)
                    {
                        await GeneratePDF(fid.NoFID, username, context, flag);
                    }

                    return (true, fid.NoFID, "Success");

                }

            }
            catch (Exception ex)
            {
                return (false, null, "Failed : " + ex.Message);
            }
        }

        public async Task<string> PushToDocman(byte[] byteFile, M_UploadFiles data, string username, string flag)
        {
            ServiceDocSoapClient docman = new ServiceDocSoapClient();
            try
            {
                string nofid_ = data.NoFID.Replace("/", "_");

                int key4 = int.Parse(DateTime.Now.ToString("ddyy"));
                var cek = Convert.ToBase64String(byteFile);

                string result_ = docman.PutFile3("E-FID", "FID_FILE", nofid_, data.UploadID, key4, "E-FID", 0,
                                        "File_" + data.Category + "-" + flag, ".pdf", "FID_FILE", username, byteFile, "File_" + data.Category);

                if (result_ != "")
                {
                    var resultDocman = JsonConvert.DeserializeObject<resultDocman>(result_);

                    var uplData = await _context.M_UploadFiles.Where(x => x.NoFID == data.NoFID && x.Category == data.Category)
                        .OrderByDescending(x => x.UploadDate).FirstOrDefaultAsync();
                    uplData.Key4 = resultDocman.Result;
                    uplData.FileName = "File_" + data.Category + "-" + flag;

                    await _context.SaveChangesAsync();
                }

                return result_;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void DeleteFromDocman(string nofid, string username)
        {
            ServiceDocSoapClient ws = new ServiceDocSoapClient();
            var data = _context.M_UploadFiles.Where(x => x.NoFID == nofid).ToList();
            
            foreach (var dt in data)
            {
                ws.DeleteFileById(Convert.ToInt32(dt.Key4), "testing.fid1", "user@default", username);
            }

            _context.M_UploadFiles.RemoveRange(data);
            _context.SaveChanges();

        }

        [Obsolete]
        public async Task<bool> GeneratePDF(string nofid, string username, ControllerContext context, string flag)
        {
            var pageOrientation = new Rotativa.Options.Orientation();
            var dataFiles = await getDocumentByFlag(nofid, flag);

            foreach (var dtFiles in dataFiles)
            {
                if (dtFiles.Category == "CBA")
                {
                    pageOrientation = Rotativa.Options.Orientation.Landscape;
                }
                else
                {
                    pageOrientation = Rotativa.Options.Orientation.Portrait;
                }

                var actionPdf = new ActionAsPdf("FormPDF", new { nofid, dtFiles.Category })
                {
                    FileName = "File_" + dtFiles.Category + "_" + flag + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,
                    PageOrientation = pageOrientation,
                    //PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                    CustomSwitches = "--footer-center \"[page]\" --footer-font-size \"10\" --footer-spacing \"5\""
                };

                byte[] byteFiles = actionPdf.BuildPdf(context);
                var strFile = Convert.ToBase64String(byteFiles);

                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "File_" + dtFiles.Category + "_" + flag + ".pdf");
                System.IO.File.WriteAllBytes(filePath, byteFiles);

                using (MemoryStream _ms = new MemoryStream())
                {
                    using (FileStream _fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        _fs.CopyTo(_ms);
                    }

                    byte[] fixFile = _ms.ToArray();
                    var string64 = Convert.ToBase64String(fixFile);

                    var sendFile = await PushToDocman(fixFile, dtFiles, username, flag);
                }

                System.IO.File.Delete(filePath);


            }

            return true;
        }

        private short setStatusSign(string flag)
        {
            if (flag == "SIG1")
            {
                return 21;
            }
            else if (flag == "SIG2")
            {
                return 22;
            }
            else if (flag == "SIG3")
            {
                return 23;
            }
            else
            {
                return 2;
            }
        }

        private async Task<dynamic> getDocumentByFlag(string nofid, string flag)
        {
            try
            {
                if (flag == "FID" || flag.StartsWith("MAN"))
                {
                    var topPS = await _context.M_UploadFiles.Where(f => f.Category == "HR" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                    var topCBA = await _context.M_UploadFiles.Where(f => f.Category == "FID" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                    var dataFiles = topPS.Union(topCBA)
                        .OrderByDescending(f => f.UploadDate)
                        .Select(x => new M_UploadFiles
                        {
                            NoFID = x.NoFID,
                            Category = x.Category,
                            HtmlTag = x.HtmlTag,
                            UploadDate = x.UploadDate,
                            UploadBy = x.UploadBy
                        });

                    return dataFiles;
                }
                else
                {
                    var topPS = await _context.M_UploadFiles.Where(f => f.Category == "PS" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                    var topCBA = await _context.M_UploadFiles.Where(f => f.Category == "CBA" && f.NoFID == nofid).OrderByDescending(f => f.UploadDate).Take(1).ToListAsync();
                    //var dataFiles = await _context.T_FIDFiles.Where(x => x.NoFID == nofid).ToListAsync();
                    var dataFiles = topPS.Union(topCBA)
                        .OrderByDescending(f => f.UploadDate)
                        .Select(x => new M_UploadFiles
                        {
                            NoFID = x.NoFID,
                            Category = x.Category,
                            HtmlTag = x.HtmlTag,
                            UploadDate = x.UploadDate,
                            UploadBy = x.UploadBy
                        });

                    return dataFiles;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public async Task UpdateEmailManagerAsync(string nofid)
        {
            try
            {
                List<M_GateReview> emailManager = new List<M_GateReview>();

                #region "Update Email Manager"
                emailManager = await _context.M_GateReview.ToListAsync();
                var data = await GetListUserByKdOrg();

                if (emailManager != null)
                {
                    var emailResult = data.Where(x => emailManager.Select(e => e.Fungsi).Contains(x.Posisi) 
                                                && x.NamaJabatan == "Manager" && !x.Username.Contains("dev") && !x.Username.Contains("mgr")).ToList();
                    var memail = await _context.M_EmailManager.ToListAsync();

                    if (emailResult != null)
                    {
                        _context.M_EmailManager.RemoveRange(memail);
                        await _context.SaveChangesAsync();

                            foreach (var dt in emailResult)
                        {
                            var M_EmailManager = new M_EmailManager
                            {
                                Jabatan = dt.NamaJabatan,
                                Fungsi = dt.Fungsi,
                                Email = dt.Email,
                                NIP = dt.NIP,
                                Username = dt.Username
                            };
                            _context.M_EmailManager.Add(M_EmailManager);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                #endregion

                #region "Add email Manager to M_Email"
                var addEmail = await _context.M_EmailManager.ToListAsync();
                var eml = await _context.M_Email.Where(x => x.NoFID == nofid && x.Status == "emailTo").ToListAsync();

                if (eml != null)
                {
                    _context.M_Email.RemoveRange(eml);
                    await _context.SaveChangesAsync();
                }

                foreach (var dtemail in addEmail)
                {
                    var m_email = new M_Email
                    {
                        NoFID = nofid,
                        Email = dtemail.Email,
                        Jabatan = dtemail.Jabatan + " " + dtemail.Fungsi,
                        Status = "emailTo",
                        CreatedBy = "SYSTEM",
                        CreatedDate = DateTime.Now
                    };
                    _context.M_Email.Add(m_email);
                }
                await _context.SaveChangesAsync();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.Message);
            }
        }

        public async Task<List<ListUser>> GetListUserByKdOrg()
        {
            try
            {
                var data = new
                {
                    username = "",
                    NIP = "",
                    KodeUnit = "",
                    IsPrimary = 1
                };

                var json = JsonConvert.SerializeObject(data);

                using (var client = new HttpClient())
                {
                    var url = ConfigurationManager.AppSettings["urlManager"].ToString();
                    client.BaseAddress = new Uri(url);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsJsonAsync(url, content);
                    var rawString = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<ListKodeUnit>(rawString);

                    return result.Data;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error = " + ex.Message);
            }
        }

        #region "Send EMAIL
        private void SendEmailAsync(string NoFID, string flag, string jenis)
        {
            try
            {
                var email = GetEmail(NoFID, flag, jenis);
                var emailcc = getEmailcc(NoFID, flag, jenis);
                string smtpAddress = ConfigurationManager.AppSettings["smtp"].ToString();
                string subject = "Approval Usulan Investasi";
                string body = getBody(flag, NoFID);

                var data = new EmailDTO
                {
                    subject = subject,
                    //to = email,
                    to = new List<string> { "rfauzi.work@gmail.com" },
                    //cc = emailcc,
                    bodyEmailHTML = body,
                };

                var jsoncontent = JsonConvert.SerializeObject(data);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(smtpAddress);
                    var content = new StringContent(jsoncontent, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(smtpAddress, content);
                    var result = response.Result.Content.ReadAsStringAsync();

                }
            }
            catch (SmtpException smtp)
            {

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private List<string> GetEmail(string nofid, string flag, string jenis)
        {
            try
            {
                List<string> listemail = new List<string>();
                var query = _context.M_Email.Where(x => x.NoFID.Equals(nofid)).ToList();

                if (flag == "NEW" || flag == "UPREV" || flag == "UPREV2")
                {
                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                          .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemail.Add(email);
                    }

                    return listemail;
                }

                if (flag == "UPREV" || flag == "UPREV2" || flag == "PGR" || flag == "APPROVE" || flag == "DRAFTFID" || flag == "REVISI" || flag == "REVISI2")
                {
                    var result = _context.M_Email.Where(x => x.NoFID.Equals(nofid) && x.Jabatan.Equals("User")).Select(x => x.Email).FirstOrDefault();
                    listemail.Add(result);

                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                          .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemail.Add(email);
                    }

                    return listemail;
                }

                if (flag == "SIGNU" || flag == "FINAL")
                {
                    var user = query.Where(x => x.Jabatan.Equals("User")).Select(x => x.Email).FirstOrDefault();
                    listemail.Add(user);

                    return listemail;
                }

                if (flag == "SIGNA")
                {
                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                           .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemail.Add(email);
                    }

                    return listemail;
                }

                if (jenis == "BD")
                {
                    if (flag == "FID")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManBPD") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANBPD")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManFBS") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANFBS")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCNL") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANCNL")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCSPRM") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    return listemail;
                }
                else
                {
                    if (flag == "FID")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCSPRM") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANCSPRM")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManFBS") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANFBS")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCNL") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }
                    else if (flag == "MANCNL")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManBPD") && x.Status.Equals("emailTo")).Select(x => x.Email).FirstOrDefault();
                        listemail.Add(result);
                    }

                    return listemail;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private List<string> getEmailcc(string nofid, string flag, string jenis)
        {
            try
            {
                List<string> listemailcc = new List<string>();
                var query = _context.M_Email.Where(x => x.NoFID.Equals(nofid)).ToList();

                if (flag == "UPREV" || flag == "UPREV2")
                {
                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                          .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemailcc.Add(email);
                    }


                    return listemailcc;
                }

                if (flag == "SIGNU" || flag == "FINAL")
                {
                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                          .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemailcc.Add(email);
                    }

                    return listemailcc;
                }

                if (jenis == "BD")
                {
                    if (flag == "FID")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManBPD") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANBPD")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManFBS") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANFBS")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCNL") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANCNL")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCSPRM") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    return listemailcc;
                }
                else if (jenis == "Non BD")
                {
                    if (flag == "FID")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManBPD") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANBPD")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManFBS") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANFBS")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCNL") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    else if (flag == "MANCNL")
                    {
                        var result = query.Where(x => x.Jabatan.Equals("ManCSPRM") && x.Status.Equals("emailCc")).Select(x => x.Email).ToList();
                        if (result.Count > 0)
                        {
                            foreach (string emailcc in result)
                            {
                                listemailcc.Add(emailcc);
                            }
                        }
                    }
                    return listemailcc;
                }
                else
                {
                    var admin = _context.M_Email.Where(x => x.Jabatan.StartsWith("Admin"))
                          .Select(x => x.Email).ToList();
                    foreach (var email in admin)
                    {
                        listemailcc.Add(email);
                    }


                    return listemailcc;
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private string getBody(string flag, string NoFID)
        {

            if (flag == "NEW")
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Telah Dibuat" +
                "<br>Silahkan klik link dibawah untuk melihat Usulan tersebut" +
                "<br><br><br><br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
            else if (flag == "UPREV" || flag == "UPREV2")
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Telah Direvisi" +
                "<br>Silahkan klik link dibawah untuk melihat Usulan tersebut" +
                "<br><br><br><br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
            else if (flag == "PGR")
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Sedang dalam Tahap GR." +
                "<br>Sistem akan menginfokan lebih lanjut apabila status Dokumen sudah berubah." +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
            else if (flag == "REVISI" || flag == "REVISI2")
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Membutuhkan Revisi.." +
                "<br>Silahkan memperbaiki dokumen susuai dengan Pesan Revisi." +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
            else if (flag == "APPROVE")
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Telah Disetujui" +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
            else if (flag == "SIGNU") //sign perisai
            {
                string body = "Usulan investasi anda sudah disetujui oleh manager terkait, " +
                "saat ini sedang dalam proses penandatanganan oleh pejabat terkait melalui PERISAI" +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>"; ;

                return body;

            }
            else if (flag == "SIGNA")
            {
                string body = "Usulan investasi anda telah disetujui oleh manager terkait, agar dilakukan upload FID Final ke PERISAI" +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>"; ;

                return body;
            }
            else if (flag == "FINAL")
            {
                string body = "Usulan investasi anda sudah disetujui oleh pejabat terkait melalui PERISAI" +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>"; ;

                return body;
            }
            else
            {
                string body = "Usulan Investasi <b>" + NoFID + "</b>" +
                "<br>Mohon untuk di review dan approval." +
                "<br>Apabila dalam waktu 1x24 jam belum direview maka <b>Sistem</b> akan melakukan Approval secara otomatis" +
                "<br><br><br><br>" +
                "Klik Link dibawah untuk melanjutkan" +
                "<br><a href='https://efid.pertaminaretail.com/'> Click Here </a>";

                return body;
            }
        }

        #endregion
    }
}