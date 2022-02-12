using System.ComponentModel.DataAnnotations;

namespace SuggestionAppUI.Models;

public class CreateSuggestionModel
{
    [Required]
    [MaxLength(75)]
    public string Suggestion { get; set; }
    
    [Required]
    [MinLength(1)]
    [Display(Name = "Category")]
    // added min length so something must be there, otherwise a blank string would be allowed with just Required
    public string CategoryId { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
}
