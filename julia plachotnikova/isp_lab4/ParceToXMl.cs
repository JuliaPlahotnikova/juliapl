using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Parce
{
    public static class ParceToXML
    {
        public static void Parce(List<Order> orders, string path)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                string xml = orders[i].ToXML();

                using (StreamWriter writer = new StreamWriter(path + $"{DateTime.Now.ToString("dd/MM/yyyy")}[{i}].xml", true))
                {
                    writer.WriteLine(xml);
                }
            }
        }
    }
}
