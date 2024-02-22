using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dajeej.Models
{
    public class FAQ
    {
        [Key]
        public int FAQId { get; set; }

        [Required]
        public string QuestionAr { get; set; }

        [Required]

        public string AnswerAr { get; set; }
        [Required]
        public string QuestionEn { get; set; }

        [Required]

        public string AnswerEn { get; set; }
    }
}
