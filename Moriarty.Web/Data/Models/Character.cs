using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public byte[] ProfilePicture { get; set; }

    [JsonIgnore]
    public ICollection<Campaign> SuspectedInCampaigns { get; set; } = [];
}