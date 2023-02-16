using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Models
{
    public class TracerSearchInput
    {
        [BindProperty]
        public int TracerSearchInputId { get; set; }

        [BindProperty]
        public string TracerSearchInputName { get; set; }

        [BindProperty]
        public string TracerSearchTransactionType { get; set; }
    }
}
