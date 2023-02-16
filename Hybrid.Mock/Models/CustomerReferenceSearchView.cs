using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hybrid.Mock.Models
{
    public class CustomerReferenceSearchView
    {
        [Display(Prompt = "Enter Customer ID here...")]
        [Required(ErrorMessage = "The Customer ID field is required for searching")]
        [BindProperty]
        public string CustomerId { get; set; }

        [Display(Prompt = "Enter Reference here...")]
        [Required(ErrorMessage = "The Reference field is required for searching")]
        [BindProperty]
        public string Reference { get; set; }
    }
}
