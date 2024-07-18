using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moriarty.Web.Data.Models;

public class Character
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public byte[] ProfilePicture { get; set; }

    public ICollection<Campaign> SuspectedInCampaigns { get; set; } = [];
}