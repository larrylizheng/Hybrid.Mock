using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hybrid.Mock.Models
{
    public class CorrelationIDSearchView
    {
        [Display(Prompt = "Enter Correlation ID here...")]
        [Required(ErrorMessage = "The Correlation ID field is required for searching")]
        [BindProperty]
        public string CorrelationID { get; set; }
    }
}
