using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using System.IO;

namespace Parce
{
    public static class ParceToJson
    {
        public static void Parce(List<Order> orders, string path)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                string json = JsonConvert.SerializeObject(orders);

                using (StreamWriter writer = new StreamWriter(path + $"{DateTime.Now.ToString("dd/MM/yyyy")}[{i}].json", true))
                {
                    writer.WriteLine(json);
                }
            }
        }
    }
}
