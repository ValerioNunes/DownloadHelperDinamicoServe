using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    [Table("tbip_helper")]
    public class IpHelperDinamico
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("tblocomotiva_id")]
        public int LocomotivaId { get; set; }
        public Locomotiva Locomotiva { get; set; }

        [Column("data")]
        public DateTime Data { get; set; }

        [Column("ip")]
        public String Ip { get; set; }
    }
}