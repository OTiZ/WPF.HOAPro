using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HOAPro.Persistence;

namespace HOAPro.Models
{
    public class HomeInvoice
    {
        public Guid DuePaymentId { get; set; }
        public string DueYear { get; set; }
        public decimal DuesAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public string CheckImageUNC { get; set; }

        public static HomeInvoice Create(DuePayment payment)
        {
            HomeInvoice hi = new HomeInvoice();
            hi.DueYear = payment.DueInvoice.DueYear.DueYear1;
            hi.DuesAmount = payment.DueInvoice.DueAmount;
            hi.PaymentAmount = payment.PaymentAmount.GetValueOrDefault(0);
            hi.CheckImageUNC = GetFullyQualifiedCheckImageUNC(payment.CheckImageUNC);
            hi.DuePaymentId = payment.DuePaymentsId;
            return hi;
        }

        private static string GetFullyQualifiedCheckImageUNC(string checkImageUNC)
        {
            if (string.IsNullOrWhiteSpace(checkImageUNC)) return string.Empty;
            string sdfFileLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string sdfFilePath = null;
            if (!string.IsNullOrWhiteSpace(sdfFileLocation))
                sdfFilePath = Path.GetDirectoryName(sdfFileLocation);
            if (!string.IsNullOrWhiteSpace(sdfFilePath))
            {
                sdfFilePath = !sdfFilePath.Trim().EndsWith(@"\") ? sdfFilePath.Trim() + @"\" : sdfFilePath.Trim();
                sdfFilePath += checkImageUNC;
                return sdfFilePath;
            }
            else
            {
                return checkImageUNC;
            }
        }
    }
}
