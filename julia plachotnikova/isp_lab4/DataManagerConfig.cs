using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DataManagerConfig
    {
        public string ConnectionString { set; get; }
        public string SourcePath { set; get; }
        public string OutputFolder { set; get; }

        public string LoggerConnectionString { set; get; }
    }
}
