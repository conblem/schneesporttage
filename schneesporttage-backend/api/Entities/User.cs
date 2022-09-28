using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities;

public class User : IEntity
{
    public string Firstname { get; set; } = "";
    public string Lastname { get; set; } = "";

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}