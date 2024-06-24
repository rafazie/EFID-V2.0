using SimplifikasiFID.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SimplifikasiFID.ServiceSSO;
using System.Text;
using System.Data.Entity;

namespace SimplifikasiFID.Services
{
    public class PMSServices
    {
        private readonly SimplyFIDEntities _context = new SimplyFIDEntities();
        private readonly SSOWSSoapClient _sso = new SSOWSSoapClient();
        ResponseFID response = new ResponseFID();

        public async Task<(bool sts, object data, string msg)> getAllDoc(string jenis_inv, DateTime startDate, DateTime endDate, string authHeader)
        {
            try
            {
                var valid = await isValid(authHeader);
                if (valid)
                {
                    if (startDate != null || endDate != null)
                    {
                        var enddates = endDate.AddDays(1);
                        var query = _context.V_FID.AsNoTracking()
                       .Where(x => x.sts == 11 && x.CreatedDate >= startDate && x.CreatedDate <= enddates && (jenis_inv == "" || x.Jenisfid == jenis_inv))
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

                        return (true, query, "Succes");
                    }
                    else
                    {
                        return (false, null, "INVALID DATETIME");
                    }
                }
                else
                {
                    return (false, null, "Unauthorized");
                }
            }
            catch (Exception ex)
            {
                return (false, null, "Error = " + ex.Message);
            }
        }

        public async Task<(bool sts, object data, string msg)> getByDocno(string docno, string authHeader)
        {
            try
            {
                var query = await isValid(authHeader);
                if (query)
                {
                    var data = await _context.V_FID.AsNoTracking().ToListAsync();
                    return (true, data, "Success");
                }
                else
                {
                    return (false, null, "Unauthorized");
                }
            }
            catch (Exception ex)
            {
                return (false, null, "Error = " + ex.Message);
            }
        }

        public async Task<bool> isValid(string authHeader)
        {
            try
            {
                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    var username = usernamePassword.Substring(0, seperatorIndex);
                    var password = usernamePassword.Substring(seperatorIndex + 1);

                    if (_sso.ValidateUser(username, password))
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
                return false;
            }
        }
    }
}