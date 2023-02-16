using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Models
{
    public class PayOutSearchInput
    {
        [BindProperty]
        public int PayOutSearchInputId { get; set; }

        [BindProperty]
        public string PayOutSearchInputName { get; set; }

        [BindProperty]
        public string PayOutSearchInputIndexName { get; set; }
    }
}
