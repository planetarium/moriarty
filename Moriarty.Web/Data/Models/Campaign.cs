using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moriarty.Web.Data.Models;

public class Campaign
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Plot { get; set; }

    [Required]
    public Guid VictimId { get; set; }

    [ForeignKey(nameof(VictimId))]
    public Character Victim { get; set; }

    [Required]
    public Guid OffenderId { get; set; }

    [ForeignKey(nameof(OffenderId))]
    public Character Offender { get; set; }

    public string Motive { get; set; }

    public string Method { get; set; }

    public ICollection<Character> Suspects { get; set; } = [];
}