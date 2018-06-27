using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TestXDocument
{
    class Program
    {
        
        static void Main(string[] args)
        {

            //CreateAgileRateShopRequest();
            //ReadAgileRateShopResponse();
            ReadAgileRateShopResponseDynamic();

        }

        public static void CreateAgileRateShopRequest()
        {
            //https://books.google.com/books?id=_Y0rWd-Q2xkC&pg=PA387&lpg=PA387&dq=c%23+XElement+xsi::nil%3D%22true%22&source=bl&ots=RaCCVaSHWl&sig=wGj-u-LEp-6SYR_66cu-iEi2y-g&hl=en&sa=X&ved=0ahUKEwi7p7edl-_bAhVJiqwKHSSPBwsQ6AEIQjAD#v=onepage&q=c%23%20XElement%20xsi%3A%3Anil%3D%22true%22&f=false
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            var nil = new XAttribute(xsi + "nil", true);
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";

            var foobar = new XDocument(
               //new XDeclaration("1.0", "utf - 16", "true"),
               new XElement("PierbridgeRateShopRequest",
                   new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                   new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                   new XElement("TransactionIdentifier", "62700"),
                   new XElement("RateGroup", "1"),
                   new XElement("RateType", "0"),
                   new XElement("ShipDate", "2018-06-15"),
                   new XElement("SaturdayDelivery", "false"),
                   new XElement("Sender",
                       new XElement("Street", "777 Lena Drive"),
                       new XElement("City", "Aurora"),
                       new XElement("Region", "OH"),
                       new XElement("PostalCode", "44202"),
                       new XElement("Country", "US"),
                       new XElement("Residential", "false"),
                       //new XElement("SaturdayDelivery", "false"),
                       new XElement("OverrideType", "OverrideAll")),
                   new XElement("Receiver",
                       new XElement("Street", "36485 Inland Valley Dr."),
                       new XElement("City", "Wildomar"),
                       new XElement("Region", "CA"),
                       new XElement("PostalCode", "92595-9700"),
                       new XElement("Country", "US"),
                       new XElement("Residential", "false")),
                   new XElement("Packages",
                       new XElement("Package",
                           new XElement("PackageType", nil),
                           new XElement("Weight", "10"),
                           new XElement("Length", "0"),
                           new XElement("Width", "0"),
                           new XElement("Height", "0"))),
                   new XElement("UserName", "PSUsers")));

            string path = Path.Combine(Path.GetTempPath(), "temp.xml");
            foobar.Save(path);
            Console.WriteLine(File.ReadAllText(path));
        }

        public static void ReadAgileRateShopResponse()
        {
            var doc = XDocument.Load(@"C:\temp\TestXsd\PierbridgeRateShopResponse.xml");

            var transactionIdentifier = doc.Descendants("TransactionIdentifier").FirstOrDefault().Value;
            var statusCode = doc.Descendants("Status").Select(x => x.Element("Code").Value).FirstOrDefault();

            var test = doc.Descendants("Status").FirstOrDefault();
            var test1 = test.Element("Code").Value;
            var test2 = test.Element("Description").Value;

            var rates = doc.Descendants("Rates");
            var rateList = rates.Descendants("Rate").ToList();
            foreach(var rate in rateList)
            {
                var carrierID = rate.Descendants("Carrier").Select(x => x.Element("ID").Value).FirstOrDefault();
                var serviceId = rate.Descendants("ServiceType").Select(x => x.Element("ID")).FirstOrDefault();
            }

            
        }

        public static void ReadAgileRateShopResponseDynamic()
        {
            var doc = XDocument.Load(@"C:\temp\TestXsd\PierbridgeRateShopResponse.xml");

            string jsonText = JsonConvert.SerializeXNode(doc);

            dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

            var transactionIdentifier = dyn.PierbridgeRateShopResponse.TransactionIdentifier;
            var statusCode = dyn.PierbridgeRateShopResponse.Status.Code;
            var Rates = dyn.PierbridgeRateShopResponse.Rates.Rate;
            foreach (var r in Rates)
            {
                var carrier = r.Carrier.ID;
                if (((IDictionary<string, object>) r.ServiceType).ContainsKey("ID"))
                {
                    var serviceType = r.ServiceType.ID;
                }

                var foo = GetExpandoProperty(r.ServiceType, "ID");


            }

        }
        
        private static string GetExpandoProperty(dynamic e, string propName)
        {
            object value;
            ((IDictionary<string, object>)e).TryGetValue(propName, out value);
            return value?.ToString();
        }
    }
}
