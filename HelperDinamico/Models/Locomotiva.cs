using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    [Table("tblocomotiva")]
    public class Locomotiva
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        public String Nome { get; set; }
    }
}