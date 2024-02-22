using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dajeej.Models
{
    public class SoicialMidiaLink
    {
        [Key]
        public int id { get; set; }
        public string facebooklink { get; set; }
        public string WhatsApplink { get; set; }
        public string LinkedInlink { get; set; }
        public string Instgramlink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
    }
}
