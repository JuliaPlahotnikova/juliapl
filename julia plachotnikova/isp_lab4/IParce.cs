using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigParser
{
    public interface IParce
    {
        T Parce<T>(string path);
    }
}
