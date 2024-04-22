using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Announcements")]    
public class Announcement
{
    [Key]
    public string Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { set; get; }

    [StringLength(250)]
    public string? Content { set; get; }

    public Guid UserId { set; get; }

    public DateTime DateCreated { set; get; }
    public DateTime DateModified { set; get; }
}