using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QR.Code.Tools;
using ManagedLzma.LZMA;
using System.Threading.Tasks;
using System.Threading;

namespace QR.Code
{
    public class PayBySquareOverkill
    {
        public string InvoiceID { get; set; } = string.Empty;
        public List<Payment> Payments { get; set; } = new List<Payment>();

        public PayBySquareOverkill()
        {
        }

        public PayBySquareOverkill(Payment payment)
        {
            Payments.Add(payment);
        }

        public PayBySquareOverkill(string iban, decimal amount, string currencyCode, string? variableSymbol, string? paymentNote)
            : this(new Payment(iban, amount, currencyCode, variableSymbol, paymentNote))
        {
        }

        public string GeneratePayBySquareOverkillString()
        {
            var ts = new TabSerializer();
            var crc = new Crc32();

            ts.Append(InvoiceID);
            ts.Append(Payments.Count);
            foreach (var payment in Payments)
            {
                ts.Append(1); // PaymentOptions: paymentorder=1, standingorder=2, directdebit=4
                ts.Append(payment.Amount.ToString().Replace(",", "."));
                ts.Append(payment.CurrencyCode);
                ts.Append(payment.PaymentDueDate);
                ts.Append(payment.VariableSymbol);
                ts.Append(payment.ConstantSymbol);
                ts.Append(payment.SpecificSymbol);
                ts.Append((string?)null); // OriginatorsReferenceInformation (VS, SS, KS in SEPA format) - only if VS, KS, SS are empty
                ts.Append(payment.PaymentNote != null && payment.PaymentNote.Length > 140
                    ? payment.PaymentNote.Substring(0, 140)
                    : payment.PaymentNote);
                ts.Append(payment.BankAccounts.Count);
                foreach (var bankAccount in payment.BankAccounts)
                {
                    ts.Append(bankAccount.IBAN);
                    ts.Append(bankAccount.BIC);
                }
                ts.Append(0); // No StandingOrderExt structure
                ts.Append(0); // No DirectDebitExt structure
                ts.Append(payment.BeneficiaryName);
                ts.Append(payment.BeneficiaryAddressLine1);
                ts.Append(payment.BeneficiaryAddressLine2);
            }

            string tabString = ts.ToString(); // TrimEnd tabs
            // Console.WriteLine(tabString.Replace("\t", "\\t"));
            byte[] value = Encoding.UTF8.GetBytes(tabString);
            // Console.WriteLine(string.Join(",", value));
            byte[] dataToCompress = crc.ComputeHash(value).Concat(value).ToArray();
            // Console.WriteLine(string.Join(",", dataToCompress));
            byte[] compressed;
            using (var inputStream = new MemoryStream(dataToCompress))
            using (var outputStream = new MemoryStream())
            using (var reader = new StreamReaderAdapter(inputStream))
            using (var writer = new StreamWriterAdapter(outputStream))
            {
                // Configure encoder to match VB LZMA parameters exactly
                var settings = new EncoderSettings
                {
                    DictionarySize = 131072, // DictSize = 131072 (128KB)
                    LC = 3, // Literal Context Bits
                    LP = 0, // Literal Position Bits
                    PB = 2, // Position Bits
                    FB = 32, // FastBytes
                    MC = 16, // Match Cycles
                    HashBytes = 4, // Hash bytes
                    FastMode = true, // Fast mode for level <= 4
                    BinaryTreeMode = false, // Not binary tree mode in fast mode
                    WriteEndMark = false // Don't write end mark
                };
                var encoder = new AsyncEncoder(settings);
                encoder.EncodeAsync(reader, writer, CancellationToken.None).GetAwaiter().GetResult();
                compressed = outputStream.ToArray();
            }
            byte[] lengthBytes = BitConverter.GetBytes((short)dataToCompress.Length);
            byte[] buff = new byte[] { 0, 0 }.Concat(lengthBytes).Concat(compressed).ToArray(); // 4x4 bits (Type=0, Version=0, DocumentType=0, Reserved=0) & 16 bit little endian length of DataToCompress (crc & value) & LzmaCompressedData
            // Console.WriteLine(string.Join(",", buff));

            return ToBase32Hex(buff);
        }

        private string ToBase32Hex(byte[] buff)
        {
            const string Base32HexCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
            var sb = new StringBuilder();

            for (int bitPos = 0; bitPos <= buff.Length * 8; bitPos += 5) // Match VB: "To Buff.Length * 8" (inclusive)
            {
                int charIndex = 0;
                for (int bi = 0; bi < 5; bi++)
                {
                    int byteIdx = (bitPos + bi) / 8;
                    int bitIdx = 7 - ((bitPos + bi) % 8);
                    byte b;
                    if (byteIdx >= buff.Length)
                    {
                        b = 0; // Do nasobku 5 bitu se doplnuje nulami na konci
                    }
                    else
                    {
                        b = buff[byteIdx];
                    }
                    if ((b & (1 << bitIdx)) != 0)
                    {
                        charIndex += 1 << (4 - bi);
                    }
                }
                sb.Append(Base32HexCharset[charIndex]);
            }

            return sb.ToString();
        }
    }
}