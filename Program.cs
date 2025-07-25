using System;
using QR.Code;

class Program
{
    static void Main()
    {
        int passed = 0, failed = 0;

        // BasicPaymentTest
        try
        {
            var gen = new PayBySquareOverkill("CZ1720100000002800266981", 1235.8m, "EUR", "654321", "PayBySquareOverkill");
            string expected = "00054000CUJ17AUG92PSOB3F1IA17JL9Q3C3SAU6AG42CGGIFROV0B3E34GR8M8UODIPGNAQ10LV7UEN52PVVLL5A6K1Q9F8TLUSUVBQG8JUN2TJSEKSR7POADRS3KF10";
            string actual = gen.GeneratePayBySquareOverkillString();
            if (actual == expected)
            {
                Console.WriteLine("BasicPaymentTest: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"BasicPaymentTest: FAIL\nExpected: {expected}\nActual:   {actual}");
                failed++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BasicPaymentTest: EXCEPTION {ex}");
            failed++;
        }

        // ComplexPaymentTest
        try
        {
            var gen = new PayBySquareOverkill();
            gen.InvoiceID = "X-4242";
            var p = new Payment();
            p.Amount = 112.35m;
            p.CurrencyCode = "EUR";
            p.BankAccounts.Add(new BankAccount("CZ1720100000002800266981", "FIOBCZPPXXX"));
            p.BankAccounts.Add(new BankAccount("CZ9120100000002000278810", "FIOBCZPPXXX"));
            p.VariableSymbol = "654321";
            p.ConstantSymbol = "0308";
            p.SpecificSymbol = "998877";
            p.PaymentNote = "PayBySquareOverkill note";
            p.BeneficiaryName = "Ing. Pavel Mikula";
            p.BeneficiaryAddressLine1 = "AddressOf(Pavel Mikula).FirstLine";
            p.BeneficiaryAddressLine2 = "AddressOf(Pavel Mikula).SecondLine";
            p.PaymentDueDate = new DateTime(2019, 1, 1);
            gen.Payments.Add(p);
            string expected = "000FC000CQQGDBVH80212G6G8MPTRE2S9TNOA5IPVOQB7V525176J4DVMIDEA6P75S6N54TM114GH1N6RF2NQKBMSI92OMEOMQNNVBSF22KQGKNP6UL6NIARGU0MR0M3KN1R3B3UUD5O68NLOJH1R00T16C8TKFS883M4H88NEHKFALKV2QLMMKC6KLQ76CDOG7BPBTUDTA4P018IPEL6M8IEFK4LR6H2HM3HS168E6MQIAG9493RVK23HF4MQ78095R25OLGLL2CORBJJVIF1MH2I08GH71O0";
            string actual = gen.GeneratePayBySquareOverkillString();
            if (actual == expected)
            {
                Console.WriteLine("ComplexPaymentTest: PASS");
                passed++;
            }
            else
            {
                Console.WriteLine($"ComplexPaymentTest: FAIL\nExpected: {expected}\nActual:   {actual}");
                failed++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ComplexPaymentTest: EXCEPTION {ex}");
            failed++;
        }

        Console.WriteLine($"\nTotal Passed: {passed}, Failed: {failed}");
    }
}
