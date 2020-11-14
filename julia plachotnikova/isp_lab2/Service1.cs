using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Security.Cryptography;

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
            object obj = new object();
            bool enabled = true;
            string target= @"D:\\TargetDirectory\";
            public Logger()
            {
                watcher = new FileSystemWatcher("D:\\ClientDirectory");
                watcher.Created += Watcher_Created;
                watcher.Deleted += Watcher_Deleted;
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
            Events(filePath);
            }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "перемещён";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void Events(string filePath)
        {
            string name = Path.GetFileName(filePath);
            string[] parts = name.Split('.');
            string fileName = parts[0];
            string text = File.ReadAllText(filePath);
            byte[] str = Encrypt(text, filePath);
            File.WriteAllText(filePath, str.ToString());
            Compress(filePath, fileName);
            Decompress(fileName);
            string texxt = File.ReadAllText(filePath);
            File.WriteAllText(target + fileName + ".txt", Decrypt(text, texxt, filePath));
            DeleteFile(filePath);
            DeleteArchive(fileName);
        }

        private void DeleteFile(string filePath)
        {
            FileInfo fileInf = new FileInfo(filePath);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }

        private void DeleteArchive(string fileName)
        {
            FileInfo fileInf = new FileInfo(target + fileName + ".gz");
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }

        private byte [] Encrypt(string text, string filePath)
        {
            
            using (Aes myAes = Aes.Create())
            {
                byte[] encrypted = EncryptStringToBytes_Aes(text, myAes.Key, myAes.IV);
                return encrypted;
            }

        }

        private string Decrypt(string text, string texxt, string filePath)
        {
            using (Aes myAes = Aes.Create())
            {
                string roundtrip = DecryptStringFromBytes_Aes(Encrypt(text, filePath), myAes.Key, myAes.IV);
                return roundtrip;
            }
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        private byte[] EncryptStringToBytes_Aes(string text, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

            private void Compress(string filePath, string fileName)
            { 
            using (FileStream sourceStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(target+fileName+".gz"))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                    }
                }
            }
            }

        private void Decompress(string fileName)
        {
            using (FileStream sourceStream = new FileStream(target + fileName + ".gz", FileMode.OpenOrCreate))
            {
                // поток для записи восстановленного файла
                using (FileStream targetStream = File.Create(target + fileName + ".txt"))
                {
                    // поток разархивации
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }
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
