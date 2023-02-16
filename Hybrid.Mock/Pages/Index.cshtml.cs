using Hyperion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Models;
using Hybrid.Mock.Utilities;

namespace Hybrid.Mock.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ITransactionService _transactionService;
    private readonly IPaymentAgreementService _paymentAgreementService;
    private readonly ISimpleStorageService _simpleStorageService;

    public IndexModel(ILogger<IndexModel> logger, ITransactionService transactionService, IPaymentAgreementService paymentAgreementService, ISimpleStorageService simpleStorageService)
    {
        _transactionService = transactionService;
        _paymentAgreementService = paymentAgreementService;
        _logger = logger;
        _simpleStorageService = simpleStorageService;
    }

    [BindProperty]
    public CustomerReferenceSearchView CustomerReferenceSearchConditions { get; set; }

    [BindProperty]
    public string IndexName { get; set; }

    [BindProperty]
    public List<PayToSearchInput> PayToSearchInputItems { get; set; }

    [BindProperty]
    public List<PayOutSearchInput> PayOutSearchInputItems { get; set; }

    [BindProperty]
    public List<TransactionTypeSearchInput> TransactionTypeSearchInputItems { get; set; }

    [BindProperty]
    public int SelectedTransactionInputItem { get; set; }

    [BindProperty]
    public int SelectedInputSearchItem { get; set; }

    [BindProperty]
    public string SelectedSearchName { get; set; }

    [BindProperty]
    public string SelectedTransactionTypeName { get; set; }

    [BindProperty]
    public TransactionSearchView TransactionSearchConditions { get; set; }

    [BindProperty]
    public CorrelationIDSearchView CorrelationIDSearchConditions { get; set; }

    [BindProperty]
    public ResourceSearchView ResourceSearchConditions { get; set; }

    public TransactionSearchItemView? TransactionSearchItem { get; set; }

    public List<TransactionTypeSearchInput> GetTransactionTypeList()
    {
        List<TransactionTypeSearchInput> transactionTypeInputs = new()
        {
            new()
            {
                TransactionTypeSearchInputId = 0,
                TransactionTypeSearchInputName = "Transaction Search"
            },
            new()
            {
                TransactionTypeSearchInputId = 1,
                TransactionTypeSearchInputName = "Customer Reference Search"
            },
            new()
            {
                TransactionTypeSearchInputId = 2,
                TransactionTypeSearchInputName = "Correlation ID Search"
            },
            new()
            {
                TransactionTypeSearchInputId = 3,
                TransactionTypeSearchInputName = "Resource Search"
            },
            new()
            {
                TransactionTypeSearchInputId = 4,
                TransactionTypeSearchInputName = "Client Reference Search"
            },
            new()
            {
                TransactionTypeSearchInputId = 5,
                TransactionTypeSearchInputName = "Correlation ID Search"
            }
        };

        return transactionTypeInputs;
    }

    public async Task<IActionResult> OnGet()
    {
        using (_logger.BeginScope("Index OnGet get transaction for summary"))
        {
            TransactionTypeSearchInputItems = GetTransactionTypeList();
            return Page();
        }
    }

    public IActionResult OnPostTransactionTypeChange()
    {
        TransactionTypeSearchInputItems = GetTransactionTypeList();
        return Page();
    }

    public async Task<IActionResult> OnPostSubmit()
    {
        try
        {
            TransactionSearchInputView searchInputView = new();

            bool isValid = false;

            switch (SelectedTransactionTypeName)
            {
                case "Pay Out Search":
                    switch (SelectedSearchName)
                    {
                        case "Customer Reference Search":
                            isValid = FormValidationHelper.ValidateCustomerReferenceSearchForm(CustomerReferenceSearchConditions);
                            if (isValid)
                            {
                                searchInputView.CustomerId = CustomerReferenceSearchConditions.CustomerId;
                                searchInputView.Reference = CustomerReferenceSearchConditions.Reference;
                            }

                            break;

                        case "Transaction Search":
                            isValid = FormValidationHelper.ValidateTransactionSearchForm(TransactionSearchConditions);
                            if (isValid)
                            {
                                searchInputView.TransactionId = TransactionSearchConditions.TransactionId;
                            }

                            break;

                        case "Correlation ID Search":
                            isValid = FormValidationHelper.ValidateCorrelationIDSearchForm(CorrelationIDSearchConditions);
                            if (isValid)
                            {
                                searchInputView.CorrelationID = CorrelationIDSearchConditions.CorrelationID;
                            }

                            break;
                    }

                    break;

                case "Pay To Search":
                    switch (SelectedSearchName)
                    {
                        case "Resource Search":
                            isValid = FormValidationHelper.ValidateResourceSearchForm(ResourceSearchConditions);
                            if (isValid)
                            {
                                searchInputView.ResourceId = ResourceSearchConditions.ResourceId;
                            }

                            break;

                        case "Client Referemce Search":
                            isValid = FormValidationHelper.ValidateCustomerReferenceSearchForm(CustomerReferenceSearchConditions);
                            if (isValid)
                            {
                                searchInputView.CustomerId = CustomerReferenceSearchConditions.CustomerId;
                                searchInputView.Reference = CustomerReferenceSearchConditions.Reference;
                            }

                            break;

                        case "Correlation ID Search":
                            isValid = FormValidationHelper.ValidateCorrelationIDSearchForm(CorrelationIDSearchConditions);
                            if (isValid)
                            {
                                searchInputView.CorrelationID = CorrelationIDSearchConditions.CorrelationID;
                            }

                            break;
                    }

                    break;
            }

            if (isValid)
            {
                using (_logger.BeginScope("Index OnPost Get Transaction Details"))
                {
                    string id = DataViewHelper.CreateID(searchInputView, SelectedSearchName);

                    switch (SelectedTransactionTypeName)
                    {
                        case "Pay Out Search":

                            var transactionSearchItem = await _transactionService.GetTransactionById(id, IndexName);

                            if (transactionSearchItem.IsSuccess && transactionSearchItem.Value != null)
                            {
                                if (transactionSearchItem.Value.CorrelationID == null || transactionSearchItem.Value.DateCreated == null)
                                {
                                    var transaction = await _transactionService.GetTransactionById(transactionSearchItem.Value.PartitionKey, "");
                                    transactionSearchItem.Value.CorrelationID = transaction.Value.CorrelationID;
                                    transactionSearchItem.Value.DateCreated = transaction.Value.DateCreated;
                                }

                                return RedirectToPage("TransactionDetails", new { Id = transactionSearchItem.Value.PartitionKey, CustomerId = transactionSearchItem.Value.SortKey, CorrelationID = transactionSearchItem.Value.CorrelationID, DateCreated = transactionSearchItem.Value.DateCreated });
                            }

                            if (SelectedSearchName == "Correlation ID Search")
                            {
                                var correlationSearchItem = await _simpleStorageService.GetAllObjectsAsync(id);

                                if (correlationSearchItem.Count != 0)
                                {
                                    string transactionDate = correlationSearchItem.OrderBy(x => x.LastModified).ElementAt(0).LastModified.ToString();
                                    return RedirectToPage("TransactionDetails", new { Id = "NA", CustomerId = "NA", CorrelationID = id, DateCreated = transactionDate });
                                }
                            }

                            break;

                        case "Pay To Search":

                            var paymentAgreementSearchItem = await _paymentAgreementService.GetPaymentAgreementById(id, IndexName);

                            if (paymentAgreementSearchItem.IsSuccess && paymentAgreementSearchItem.Value != null)
                            {
                                if (paymentAgreementSearchItem.Value.CorrelationID == null || paymentAgreementSearchItem.Value.CreatedDatetime == null)
                                {
                                    var paymentAgreement = await _paymentAgreementService.GetPaymentAgreementById(paymentAgreementSearchItem.Value.pk, "");
                                    paymentAgreementSearchItem.Value.CorrelationID = paymentAgreement.Value.CorrelationID;
                                    paymentAgreementSearchItem.Value.CreatedDatetime = paymentAgreement.Value.CreatedDatetime;
                                }

                                return RedirectToPage("PaymentAgreementDetails", new { Id = paymentAgreementSearchItem.Value.pk, CustomerId = paymentAgreementSearchItem.Value.sk, CorrelationID = paymentAgreementSearchItem.Value.CorrelationID, DateCreated = paymentAgreementSearchItem.Value.CreatedDatetime });
                            }

                            if (SelectedSearchName == "Correlation ID Search")
                            {
                                var correlationSearchItem = await _simpleStorageService.GetAllObjectsAsync(id);

                                if (correlationSearchItem.Count != 0)
                                {
                                    string paymentAgreementDate = correlationSearchItem.OrderBy(x => x.LastModified).ElementAt(0).LastModified.ToString();
                                    return RedirectToPage("TransactionDetails", new { Id = "NA", CustomerId = "NA", CorrelationID = id, DateCreated = paymentAgreementDate });
                                }
                            }

                            break;
                    }
                }
            }
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                _logger.LogError(e, "Task error calling OnPostSubmit Index method: {message}", e.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not find transaction data: {message}", ex.Message);
        }

        ModelState.AddModelError("", $"Could not find transaction data using {SelectedSearchName} conditions provided.");

        TransactionTypeSearchInputItems = GetTransactionTypeList();

        return Page();
    }
}