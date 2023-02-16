using Hybrid.Mock.Models;

namespace Hybrid.Mock.Utilities
{
    public static class FormValidationHelper
    {
        public static bool ValidateCustomerReferenceSearchForm(CustomerReferenceSearchView customerReferenceSearchConditions)
        {
            if (String.IsNullOrEmpty(customerReferenceSearchConditions.CustomerId) || String.IsNullOrEmpty(customerReferenceSearchConditions.Reference)) { return false; }
            return true;
        }

        public static bool ValidateTransactionSearchForm(TransactionSearchView transactionSearchConditions)
        {
            if (String.IsNullOrEmpty(transactionSearchConditions.TransactionId)) { return false; }
            return true;
        }

        public static bool ValidateCorrelationIDSearchForm(CorrelationIDSearchView correlationIDSearchConditions)
        {
            if (String.IsNullOrEmpty(correlationIDSearchConditions.CorrelationID)) { return false; }
            return true;
        }

        public static bool ValidateResourceSearchForm(ResourceSearchView resourceSearchConditions)
        {
            if (String.IsNullOrEmpty(resourceSearchConditions.ResourceId)) { return false; }
            return true;
        }
    }
}
