using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HOAPro.Persistence;

namespace HOAPro.Models
{
    public class DueInvoiceOwed
    {
        public Guid DueInvoicesId { get; set; }
        public string DueYear { get; set; }
        public decimal DuesOwed { get; set; }

        public static DueInvoiceOwed Create(Persistence.DueInvoice di)
        {
            DueInvoiceOwed dio = new DueInvoiceOwed();
            dio.DueInvoicesId = di.DueInvoicesId;
            dio.DueYear = di.DueYear.DueYear1;
            var paid = di.DuePayments == null ? 0 : di.DuePayments.Sum(d => d.PaymentAmount.GetValueOrDefault(0));
            dio.DuesOwed = di.DueAmount - paid;
            return dio;
        }

        public override string ToString()
        {
            return (this.DueYear ?? string.Empty).Trim() + " - " + this.DuesOwed.ToString("c") + " owed";
        }
    }
}
