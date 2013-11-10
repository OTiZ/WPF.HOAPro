using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace HOAPro.Persistence
{
    public class Persistence
    {
        public static HOAProDBContainer CreateContext()
        {
            HOAProDBContainer context = null;
            if (System.Reflection.Assembly.GetExecutingAssembly() != null)
            {
                string sdfFileLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string sdfFilePath = null;
                if(!string.IsNullOrWhiteSpace(sdfFileLocation))
                    sdfFilePath = Path.GetDirectoryName(sdfFileLocation);
                if (!string.IsNullOrWhiteSpace(sdfFilePath))
                {
                    sdfFilePath = !sdfFilePath.Trim().EndsWith(@"\") ? sdfFilePath.Trim() + @"\" : sdfFilePath.Trim();
                    sdfFilePath += @"Persistence\HOAPro.sdf";
                    if (System.IO.File.Exists(sdfFilePath))
                    {
                        //string providerConnectionString = @"metadata=res://*/Persistence.HOAPro.csdl|res://*/Persistence.HOAPro.ssdl|res://*/Persistence.HOAPro.msl;provider=System.Data.SqlServerCe.3.5;provider connection string=""Data Source=" + sdfFilePath + @""";";
                        string providerConnectionString = string.Format(@"metadata=res://*/Persistence.HOAPro.csdl|res://*/Persistence.HOAPro.ssdl|res://*/Persistence.HOAPro.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}"";", sdfFilePath);
                        context = new HOAProDBContainer(providerConnectionString);
                    }
                }
            }
            if (context == null)
                context = new HOAProDBContainer();
            return context;
        }
    }

    public partial class Home
    {
        public override string ToString()
        {
            return this.PhysicalAddress;
        }
    }

    public partial class DueYear
    {
        public override string ToString()
        {
            return this.DueYear1;
        }

        public DueYear()
        {
            this.DuesDueDate = DateTime.Now;
        }
    }

    public partial class DueInvoice
    {
        public decimal TotalPayment
        {
            get
            {
                if (this.DuePayments == null)
                    return 0;
                else
                    return this.DuePayments.Sum(d => d.PaymentAmount.GetValueOrDefault(0));
            }
        }
    }
}
