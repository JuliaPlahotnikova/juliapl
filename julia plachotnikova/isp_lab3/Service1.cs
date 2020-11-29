using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using ClassLibraryForWS;

namespace MyWindowsService
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            logger = new Logger();
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }

    class Logger
    {
        FileSystemWatcher watcher;
        EtlXmlJsonOption options;
        ParsOptions pmanager;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            if (File.Exists(@"C:\Users\Admin\source\repos\mywindowsservice\bin\Debug\netcoreapp3.1\mws.json"))
            {
                pmanager = new ParsOptions(@"C:\Users\Admin\source\repos\mywindowsservice\bin\Debug\netcoreapp3.1\mws.json");
            }
            else
            {
                pmanager = new ParsOptions(
                    @"C:\Users\Admin\source\repos\mywindowsservice\bin\Debug\netcoreapp3.1\config.xml");
            }

            options = pmanager.GetModel<EtlXmlJsonOption>();
            var path = options.pathes.ClientDirectory;
            watcher = new FileSystemWatcher(path);
            watcher.Created += Watcher_Created;
            watcher.Filter = "*" + options.cryptOptions.Extension;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine(String.Format("{0} файл {1} был {2}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                    writer.Flush();
                }
            }
        }
    }
}