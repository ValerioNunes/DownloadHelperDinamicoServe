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
    public class RacContext : DbContext
    {

        public RacContext() : base("name=RacContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public virtual IDbSet<Evento> Eventos { get; set; }
        //public virtual IDbSet<TreinamentoRac> TreinamentosRac { get; set; }
        //public virtual IDbSet<HistoricoRac> HistoricosRac { get; set; }
        //public virtual IDbSet<SedeMap> SedesMap { get; set; }
    }
}