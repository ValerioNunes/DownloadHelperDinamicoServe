using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    public class EOTLog
    {
        public int  Id { get; set; }
        public int  BateriaMinina { get; set; }
        public long TempoBateriaMinina { get; set; }
    }
}