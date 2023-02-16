using Hybrid.Mock.Models;

namespace Hybrid.Mock.Utilities
{
        public static class DataViewHelper
    {
        #region public methods
        public static string RemovePrefixBeforeHash(string data)
        {
            return data.Split("#").Last();
        }

        public static string AddPrefixToData(string data, DataType type)
        {
            var prefix = GetPrefix(type);
            return $"{prefix}{data}";
        }

        public static string CreateID(TransactionSearchInputView searchConditions, string searchType)
        {
            string id = string.Empty;

            switch (searchType)
            {
                case "Customer Reference Search":
                    if (!string.IsNullOrEmpty(searchConditions.CustomerId)) { id += CreateCustomerID(searchConditions); }
                    if (!string.IsNullOrEmpty(searchConditions.Reference)) { id += CreateReferenceID(searchConditions); }
                    break;
                case "Transaction Search":
                    if (!string.IsNullOrEmpty(searchConditions.TransactionId)) { id += CreateTransactionID(searchConditions); }
                    break;
                case "Correlation ID Search":
                    if (!string.IsNullOrEmpty(searchConditions.CorrelationID)) { id += searchConditions.CorrelationID; }
                    break;
                case "Resource Search":
                    if (!string.IsNullOrEmpty(searchConditions.ResourceId)) { id += CreateResourceID(searchConditions); }
                    break;
            }

            return id;
        }

        private static string CreateTransactionID(TransactionSearchInputView transactionConditions)
        {
            return DataViewHelper.AddPrefixToData(transactionConditions.TransactionId.Trim(), DataViewHelper.DataType.Transaction);
        }

        private static string CreateCustomerID(TransactionSearchInputView customerReferenceConditions)
        {
            return DataViewHelper.AddPrefixToData(customerReferenceConditions.CustomerId.Trim(), DataViewHelper.DataType.Customer);
        }

        private static string CreateReferenceID(TransactionSearchInputView referenceConditions)
        {
            return DataViewHelper.AddPrefixToData(referenceConditions.Reference.Trim(), DataViewHelper.DataType.Reference);
        }

        private static string CreateResourceID(TransactionSearchInputView resourceConditions)
        {
            return DataViewHelper.AddPrefixToData(resourceConditions.ResourceId.Trim(), DataViewHelper.DataType.Resource);
        }

        public enum DataType
        {
            Transaction,
            Customer,
            Reference,
            Resource
        }

        #endregion

        #region private methods

        private static string GetPrefix(DataType type)
        {
            var prefix = type switch
            {
                DataType.Transaction => "TXN#",
                DataType.Customer => "CUST#",
                DataType.Reference => "REF#",
                DataType.Resource => "PA#",
                _ => ""
            };
            return prefix;
        }
        #endregion

    }
}
