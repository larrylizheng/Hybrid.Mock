using Microsoft.AspNetCore.Mvc;

namespace Hybrid.Mock.Models
{
    public class TransactionTypeSelect
    {
        [BindProperty]
        public int TransactionTypeId { get; set; }

        [BindProperty]
        public string TransactionTypeName { get; set; }
    }
}
