using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    public class InfoLog
    {
        public int Id { get; set; }

        public String Nome { get; set; }

        public String Descricao { get; set; }

        public String Ativo { get; set; }

        public DateTime Data { get; set; }

        public String Log { get; set; }

    }
}