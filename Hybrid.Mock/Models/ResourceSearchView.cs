using System.ComponentModel.DataAnnotations;

namespace Hybrid.Mock.Models
{
    public class ResourceSearchView
    {
        [Required(ErrorMessage = "The Resource ID field is required for searching")]
        [Display(Prompt = "Enter Resource ID here...")]
        public string ResourceId { get; set; }
    }
}
