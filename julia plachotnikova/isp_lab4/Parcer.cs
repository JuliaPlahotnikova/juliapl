using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigParser
{
    public class Parcer<T>
    {
        public IParce Parce;

        public T Config { set; get; }

        public Parcer(string configPath)
        {
            if (Path.GetExtension(configPath) == "XML")
            {
                Parce = new XMLParcer();
            }
            else
            {
                Parce = new JsonParcer();
            }

            Config = Parce.Parce<T>(configPath);
        }
    }
}
