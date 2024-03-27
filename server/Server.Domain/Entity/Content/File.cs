﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Server.Domain.Entity.Content
{
    [Table("Files")]
    public class File : BaseEntity
    {
        [Required]
        public Guid ContributionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Type { get; set; }
        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
    }
}
