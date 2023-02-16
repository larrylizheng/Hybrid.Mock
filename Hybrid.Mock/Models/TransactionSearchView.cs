using System.ComponentModel.DataAnnotations;

namespace Hybrid.Mock.Models
{
    public class TransactionSearchView
    {
        [Required(ErrorMessage = "The Transaction ID field is required for searching")]
        [Display(Prompt = "Enter Transaction ID here...")]
        public string TransactionId { get; set; }
    }
}
