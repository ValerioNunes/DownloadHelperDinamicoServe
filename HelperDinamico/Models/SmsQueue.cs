using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelperDinamico.Models
{
    [Table("tbsmsqueue")]
    public class SmsQueue
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("sender")]
        public string Sender { get; set; }
        [Column("receiver")]
        public string Receiver { get; set; }
        [Column("subject")]
        public string Subject { get; set; }
        [Column("msg")]
        public string Message { get; set; }
        [Column("app_info")]
        public string AppInfo { get; set; }
        [Column("hist")]
        public DateTime Hist { get; set; }
        [Column("attachment")]
        public string Attachment { get; set; }
    }
}