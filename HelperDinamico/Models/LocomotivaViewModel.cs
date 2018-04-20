using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    public class LocomotivaViewModel
    {
        public string Nome { get; set; }
        public List<FileInfo> Arquivos { get; set; }
    }
}