using HelperDinamico.Models;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace HelperDinamico.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class IpHelperDinamicoContext : DbContext
    {
        public IpHelperDinamicoContext() : base("name=IpHelperDinamicoContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public virtual IDbSet<IpHelperDinamico> IpHelperDinamico { get; set; }
        public virtual IDbSet<Locomotiva> Locomotiva { get; set; }
    }
}