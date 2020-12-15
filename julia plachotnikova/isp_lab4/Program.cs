using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ConfigParser;
using Models;
using DataManager.Repository.Implementation;
using System.Data.SqlClient;
using System.Data;
using Parce;
using System.IO;
using ServiceLayer;

namespace lab4
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {
            Service1 myService = new Service1();
            myService.onDebug();
            string configPath = @"C:\Users\Julia\source\repos\lab4\config\";
            string[] arr = Directory.GetFiles(configPath);
            Parcer<DataManagerConfig> parcer = new Parcer<DataManagerConfig>(arr[0]);
            DataManagerConfig config = parcer.Config;
            var service = new Services(config.ConnectionString);

            var result = service.GetInfo(5);
            List<Order> list = result.ToList<Order>();

            if(Path.GetExtension(configPath) == "XML")
            {
                ParceToXML.Parce(list, config.SourcePath);
            }
            else
            {
                ParceToJson.Parce(list, config.SourcePath);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
