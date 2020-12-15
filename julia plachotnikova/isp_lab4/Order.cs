using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShipCountry { get; set; }
        public string ShipCity { get; set; }
        public string ShipAddress { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public string ToXML()
        {
            return 
            ($"<?xml version={"1.0"}?>\n" +
             "<Data>\n" +   
             $"    <OrderID>{OrderID}</OrderID>\n" +
             $"    <FirstName>{FirstName}</FirstName>\n" +
             $"    <LastName>{LastName}</LastName>\n" +
             $"    <ShipCountry>{ShipCountry}</ShipCountry>\n" +
             $"    <ShipCity>{ShipCity}</ShipCity>\n" +
             $"    <ShipAddress>{ShipAddress}</ShipAddress>\n" +
             $"    <Freight>{Freight}</Freight>\n" +
             $"    <ShipName>{ShipName}</ShipName>\n" +
             $"    <ContactName>{ContactName}</ContactName>\n" +
             $"    <CompanyName>{CompanyName}</CompanyName>\n" +
             $"    <Phone>{Phone}</Phone>\n" +
             "</Data>\n");                
        }
    }
}
