using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace Dajeej.Models
{
    public class PageContent
    {
        [Key]
        public int PageContentId { get; set; }

        [Required]
        public string PageTitleAr { get; set; }
        [Required]
        public string ContentAr { get; set; }
        [Required]
        public string PageTitleEn { get; set; }
        [Required]
        public string ContentEn { get; set; }
    }
}