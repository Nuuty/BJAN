namespace WCFServiceWebRole1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbC : DbContext
    {
        public DbC()
            : base("name=connectionstring")
        {
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
