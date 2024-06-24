using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimplifikasiFID.Models;
using SimplifikasiFID.ServiceSSO;
using SimplifikasiFID.ServiceDOC;
using System.Data;
using SimplifikasiFID.Classes;

namespace SimplifikasiFID.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Logout()
        {
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SignOn(FormCollection form)
        {
            SSOWSSoapClient ws = new SSOWSSoapClient();
            if (ws.ValidateUser(form["Username"], form["Password"]))
            {
                FormsAuthentication.SetAuthCookie(form["Username"], false);
                HttpCookie cookie = new HttpCookie("ErrorCookie", "");
                Response.SetCookie(cookie);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpCookie cookie = new HttpCookie("ErrorCookie", "Invalid Login");
                Response.SetCookie(cookie);
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult GetRole(FormCollection form)
        {
            try
            {
                string ContractId = form["ContractId"];
                string SQL = "exec FMS.dbo.procGetAllowActionCurrect_E_Kontrak_Procurement @ContractId = '" + ContractId + "',@UserName = '" + User.Identity.Name.ToString() + "'";

                string _allow_button = string.Empty;
                string AllowAddPage = string.Empty;
                string AllowDeletePage = string.Empty;
                string AllowSubmit = string.Empty;
                string AllowReject = string.Empty;
                string AllowAddAttachment = string.Empty;
                string AllowDownloadDraft = string.Empty;

                DataTable tblData = Koneksi.GetDataTable(SQL);

                if (int.Parse(tblData.Rows[0]["AllowAddPage"].ToString()) >= 1) AllowAddPage = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools\" onclick=\"add_new_page(); \"><i class=\"ace-icon fa fa-file\" aria-hidden=\"true\" style=\"color:orange; \"></i> New Page</button>";
                if (int.Parse(tblData.Rows[0]["AllowDeletePage"].ToString()) >= 1) AllowDeletePage = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools\" onclick=\"delete_page(); \"><i class=\"ace-icon fa fa-trash\" aria-hidden=\"true\" style=\"color:red; \"></i> Delete Page</button>";
                if (int.Parse(tblData.Rows[0]["AllowSubmit"].ToString()) >= 1) AllowSubmit = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools btn_process\" onclick=\"prep_save_draft()\"><i class=\"ace-icon fa fa-floppy-o\" aria-hidden=\"true\" style=\"color: dodgerblue; \"></i> Process to Next Step</button>";
                if (int.Parse(tblData.Rows[0]["AllowReject"].ToString()) >= 1) AllowReject = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools btn_reject_draft\" onclick=\"prep_save_draft('reject')\"><i class=\"ace-icon fa fa-undo\" aria-hidden=\"true\" style=\"color:red;\"></i> Reject</button>";
                if (int.Parse(tblData.Rows[0]["AllowAddAttachment"].ToString()) >= 1) AllowAddAttachment = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools btn-clear-attach btn-attach-info\" hidden>...</button> <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools btn-clear-attach\" onclick=\"clear_attach_file()\" hidden><i class=\"ace-icon fa fa-trash\" aria-hidden=\"true\"></i> Clear Attachment File</button> <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools\" onclick=\"$('#attachment_draft_e_kontrak').click();\"><i class=\"ace-icon fa fa-plus-circle\" aria-hidden=\"true\" style=\"color:blue;\"></i> Tambahakan Lampiran</button>";
                if (int.Parse(tblData.Rows[0]["AllowDownloadDraft"].ToString()) >= 1) AllowDownloadDraft = " <button type=\"button\" class=\"btn btn-white btn-bold btn-draft-tools btn_download_draft\" onclick=\"download_draft()\"><i class=\"ace-icon fa fa-file-pdf-o\" aria-hidden=\"true\" style=\"color:red;\"></i> Download Draft PDF </button>";

                _allow_button = AllowAddAttachment + AllowDownloadDraft + AllowAddPage + AllowDeletePage + AllowReject + AllowSubmit;

                return Content(_allow_button);
            }
            catch (Exception ex)
            {
                return Content("Error : " + ex.Message.ToString());
            }
        }
    }
}