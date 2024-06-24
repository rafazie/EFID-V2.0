using SimplifikasiFID.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace SimplifikasiFID.Models
{
    public class SimplyFIDEntitiesss : DbContext
    {
        public DbSet<M_Email> M_Email { get; set; }
        public DbSet<M_EmailManager> M_EmailManager { get; set; }
        public DbSet<M_Fungsi> M_Fungsi { get; set; }
        public DbSet<M_Status> M_Status { get; set; }
        public DbSet<M_TemplateForm> M_TemplateForm { get; set; }
        public DbSet<M_UploadFiles> M_UploadFiles { get; set; }
        public DbSet<M_User> M_User { get; set; }
        public DbSet<T_Catatan> T_Catatan { get; set; }
        public DbSet<T_FID> T_FID { get; set; }
        public DbSet<vFID> vFID { get; set; }
        public DbSet<T_DocFID> T_DocFID { get; set; }

        public SimplyFIDEntitiesss()
      : base("name=EntitiesConnection")
        {
        }
    }
}