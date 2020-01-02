using MySql.Data.Entity;
using HelperDinamico.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace HelperDinamico.Dal
{
    //[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class UserContext : DbContext
    {
        public UserContext() : base("name=SMSContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }       
        
        public virtual IDbSet<SmsQueue> SmsQueue { get; set; }
    }
}