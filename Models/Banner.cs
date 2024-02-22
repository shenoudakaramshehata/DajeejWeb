using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Dajeej.Models
{
    public class Banner
    {
        [Key]
        public int BannerId { get; set; }
        public string Pic { get; set; }
        public bool? IsActive { get; set; }
        [Required]
        public int? OrderIndex { get; set; }
        [Required]
        public int? EntityTypeNotifyId { get; set; }
        [Column("EntityID")]
        [StringLength(50)]
        public string EntityId { get; set; }
        [NotMapped]
        [Required]
        public IFormFile Picture { get; set; }

        public int? CountryId { get; set; }

        [ForeignKey("CountryId")]
        [JsonIgnore]
        public virtual Country Country { get; set; }
        [JsonIgnore]
        public virtual EntityTypeNotify EntityTypeNotify { get; set; }

    }
}