using System.ComponentModel.DataAnnotations;

namespace Moriarty.Web.Data.Models;

public class Character
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public byte[] ProfilePicture { get; set; }

    public ICollection<Campaign> SuspectedInCampaigns { get; set; } = [];
}