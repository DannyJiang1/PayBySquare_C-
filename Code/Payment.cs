using System;
using System.Collections.Generic;

namespace QR.Code
{
    public class Payment
    {
        // Mandatory
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        // Optional
        public string? VariableSymbol { get; set; }
        public string? ConstantSymbol { get; set; }
        public string? SpecificSymbol { get; set; }
        public string? PaymentNote { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryAddressLine1 { get; set; }
        public string? BeneficiaryAddressLine2 { get; set; }
        public DateTime? PaymentDueDate { get; set; }

        public Payment()
        {
        }

        public Payment(string iban, decimal amount, string currencyCode, string? variableSymbol, string? paymentNote)
        {
            BankAccounts.Add(new BankAccount(iban));
            Amount = amount;
            CurrencyCode = currencyCode;
            VariableSymbol = variableSymbol;
            PaymentNote = paymentNote;
        }
    }
}