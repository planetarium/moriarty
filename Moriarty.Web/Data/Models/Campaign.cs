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

    public string OpenAIFileId { get; set;}

    public ICollection<Character> Suspects { get; set; } = [];

    public string LLMInstruction
    {
        get
        {
            var instruction = $"Campaign Title: {Title}. ";

            if (!string.IsNullOrEmpty(Plot))
            {
                instruction += $"Plot: {Plot}. ";
            }

            instruction += $"Victim: {Victim?.Name ?? "Unknown"} (Description: {Victim?.Description ?? "N/A"}, Age: {Victim?.Age}). ";
            instruction += $"Offender: {Offender?.Name ?? "Unknown"} (Description: {Offender?.Description ?? "N/A"}, Age: {Offender?.Age}). ";

            if (!string.IsNullOrEmpty(Motive))
            {
                instruction += $"Motive: {Motive}. ";
            }

            if (!string.IsNullOrEmpty(Method))
            {
                instruction += $"Method: {Method}. ";
            }

            if (Suspects.Any())
            {
                List<string> suspectDescriptions = Suspects.Select(s => $"{s.Name} (Description: {s.Description}, Age: {s.Age})").ToList();
                instruction += $"Suspects: {string.Join(", ", suspectDescriptions)}.";
            }

            return instruction;
        }
    }
}