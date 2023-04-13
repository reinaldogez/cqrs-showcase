namespace CqrsShowCase.Query.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Post")]
public class PostEntity
{
    [Key]
    public Guid PostId { get; set; }
    public string Author { get; set; }
    public DateTime DatePosted { get; set; }
    public string Message { get; set; }
    public int Likes { get; set; }
    public virtual ICollection<CommentEntity> Comments { get; set; }
}