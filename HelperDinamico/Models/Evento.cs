using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    [Table("evento")]
    public class Evento
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("SistemaId")]
        public int SistemaId { get; set; }

        [Column("InicioAvaria")]
        public DateTime InicioAvaria { get; set; }

        [Column("FimAvaria")]
        public DateTime FimAvaria { get; set; }

        [Column("DuracaoParada")]
        public String DuracaoParada { get; set; }

        [Column("Descricao")]
        public String Descricao { get; set; }

        [Column("DescricaoDetalhada")]
        public String DescricaoDetalhada { get; set; }

        [Column("Sala")]
        public String Sala { get; set; }

        [Column("Nota")]
        public String Nota { get; set; }

        [Column("Ordem")]
        public String  Ordem { get; set; }

        [Column("CampoOrdenacao")]
        public String CampoOrdenacao { get; set; }
    }
}