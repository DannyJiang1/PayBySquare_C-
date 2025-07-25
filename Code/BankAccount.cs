using System;

namespace QR.Code
{
    public class BankAccount
    {
        public string IBAN { get; set; }
        public string? BIC { get; set; }

        public BankAccount(string iban, string? bic = null)
        {
            IBAN = iban;
            BIC = bic;
        }
    }
}