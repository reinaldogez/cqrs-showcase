namespace CqrsShowCase.Query.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Comment")]
public class CommentEntity
{
    [Key]
    public Guid CommentId { get; set; }
    public string Username { get; set; }
    public DateTime CommentDate { get; set; }
    public string Comment { get; set; }
    public bool Edited { get; set; }
    public Guid PostId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PostEntity Post { get; set; }
}
