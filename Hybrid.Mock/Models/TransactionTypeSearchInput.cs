using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Models
{
    public class TransactionTypeSearchInput
    {
        [BindProperty]
        public int TransactionTypeSearchInputId { get; set; }

        [BindProperty]
        public string TransactionTypeSearchInputName { get; set; }

        [BindProperty]
        public string TransactionTypeSearchInputIndexName { get; set; }
    }
}
