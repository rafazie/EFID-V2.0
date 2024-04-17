using EFID_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace EFID_V2.Models
{
    public class EFIDContext : DbContext
    {
        IHttpContextAccessor _httpContextAccessor;
        public EFIDContext(DbContextOptions<EFIDContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<M_Email> M_Email { get; set; }
        public DbSet<M_EmailManager> M_EmailManager { get; set; }
        public DbSet<M_Fungsi> M_Fungsi { get; set; }
        public DbSet<M_Status> M_Status { get; set; }
        public DbSet<M_UploadFiles> M_UploadFiles { get; set; }
        public DbSet<M_User> M_User { get; set; }
        public DbSet<T_Catatan> T_Catatan { get; set; }
        public DbSet<T_FID> T_FID { get; set; }
        public DbSet<vFID> vFID { get; set; }
    }
}
