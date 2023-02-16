using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Models
{
    public class PayToSearchInput
    {
        [BindProperty]
        public int PayToSearchInputId { get; set; }

        [BindProperty]
        public string PayToSearchInputName { get; set; }

        [BindProperty]
        public string PayToSearchInputIndexName { get; set; }
    }
}
