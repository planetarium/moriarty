using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Moriarty.Web.Data.Models;

public class Clue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    [JsonIgnore]
    public Campaign Campaign { get; set; }

    [JsonIgnore]
    public Guid CampaignId { get; set; }
}