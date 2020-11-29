using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;


namespace ClassLibraryForWS
{
    public class MainPathes
    {

        public string ClientDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public string ArchFolder { get; set; }
    }

    public class CryptOptions
    {
        public string Extension { get; set; }
        public string Cryptext { get; set; }

        public string Path1(string path)
        {
            var tpath = path.Remove(path.Length - Extension.Length) + Cryptext;

            return tpath;
        }
        public byte[] Encrypt(string path)
        {

            using (Aes myAes = Aes.Create())
            {
                string text = File.ReadAllText(path);
                byte[] encrypted = EncryptStringToBytes_Aes(text, myAes.Key, myAes.IV);
                return encrypted;
            }

        }
        public byte[] EncryptStringToBytes_Aes(string text, byte[] Key, byte[] IV)
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
        public string Decrypt(string text, string texxt, string path)
        {
            using (Aes myAes = Aes.Create())
            {
                string roundtrip = DecryptStringFromBytes_Aes(Encrypt(path), myAes.Key, myAes.IV);
                return roundtrip;
            }
        }
        public string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
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
    }

    public class ComDecOptions
    {
        public string Cryptextension { get; set; }

        public string Compress(string path)
        {
            var paths = new CryptOptions();
            var tpath = paths.Path1(path);
            var stpath = tpath.Remove(tpath.Length - Cryptextension.Length) + ".gz";
            using (FileStream sourceStream = new FileStream(tpath, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(stpath))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                    }
                }
            }

            return stpath;
        }

        public string Decompress(string newpath)
        {
            string newtargetpath = newpath.Remove(newpath.Length - 3) + Cryptextension;
            using (FileStream sourceStream = new FileStream(newpath, FileMode.OpenOrCreate))
            {
                // поток для записи восстановленного файла
                using (FileStream targetStream = File.Create(newtargetpath))
                {
                    // поток разархивации
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
                File.Delete(newpath);
            }
            return newtargetpath;
        }
    }

    public class MoveOptions
    {
        public char w { get; set; }

        public string Move(string stpath, string Targetpath)
        {

            var i = stpath.Length - 1;
            while (w != stpath[i]) i--;
            var name = stpath.Substring(i);
            var newpath = Path.Combine(Targetpath, name);
            FileInfo fileInf = new FileInfo(stpath);
            if (fileInf.Exists)
            {
                fileInf.CopyTo(newpath, true);
            }

            return newpath;
        }
    }

    public class EtlXmlJsonOption
    {

        public MainPathes mpathes { get; set; }
        public ComDecOptions archivizeOptions { get; set; }
        public CryptOptions cryptOptions { get; set; }
        public MoveOptions moveOptions { get; set; }

        public void Do(string filePath)
        {
            byte[] str = cryptOptions.Encrypt(filePath);
            File.WriteAllText(cryptOptions.Path1(filePath), str.ToString());
            var archstr = archivizeOptions.Compress(filePath);
            var newstr = moveOptions.Move(archstr, mpathes.ClientDirectory);
            var newcrypt = archivizeOptions.Decompress(newstr);
            var getstr = cryptOptions.Decrypt(File.ReadAllText(filePath), File.ReadAllText(newcrypt), filePath);
            var delfile = newcrypt;
            newcrypt = newcrypt.Remove(newcrypt.Length - 5) + ".txt";
            File.WriteAllText(newcrypt, getstr);
            File.Delete(delfile);

            string folder = Path.Combine(mpathes.ClientDirectory, mpathes.ArchFolder);
            var stpath = getstr.Remove(getstr.Length - cryptOptions.Extension.Length) + ".gz";
            var i = stpath.Length - 1;
            while (moveOptions.w != stpath[i]) i--;
            var name = stpath.Substring(i);
            var newpath = Path.Combine(folder, name);



            using (FileStream sourceStream = new FileStream(getstr, FileMode.Open))
            {
                using (FileStream targetStream = File.Create(newpath))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }
    }
    public class ParsOptions
    {
        private string path;

        public ParsOptions(string name)
        {
            path = name;
        }

        public T GetModel<T>()
        {
            var i = path.Length - 1;
            while (path[i] != '.') --i;
            var ext = path.Substring(i);
            if (ext == ".json")
            {
                IParser parser = new jsonParser(path, typeof(T));
                return parser.GetOptions<T>();
            }
            else
            {
                IParser parser = new XmlParser(path, typeof(T));
                return parser.GetOptions<T>();
            }
        }
    }

    //for parsers
    interface IParser
    {
        public T GetOptions<T>();
    }
    //json parser
    class jsonParser : IParser
    {
        private Type classs;
        private string path;

        public jsonParser(string p, Type tipe)
        {
            path = p;
            classs = tipe;
        }

        public T GetOptions<T>()
        {
            object result = Activator.CreateInstance(typeof(T));

            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();


                foreach (PropertyInfo pi in properties)
                {


                    DeserializeRecursive(pi, result, FindElement<T>());
                }
            }
            catch (Exception)
            {
                result = null;

            }



            return (T)result;
        }

        private JsonElement FindElement<T>()
        {
            string json;
            using (StreamReader file = File.OpenText(path))
            {
                json = file.ReadToEnd();
            }

            JsonDocument doc = JsonDocument.Parse(json);

            if (typeof(T) == classs)
            {
                return doc.RootElement;
            }

            PropertyInfo[] properties = classs.GetProperties();

            JsonElement result = default;

            foreach (PropertyInfo pi in properties)
            {
                FindElementnotRecursive<T>(pi, doc.RootElement, ref result);
            }

            JsonElement d = default;
            if (result.Equals(d))
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }

        private void FindElementnotRecursive<T>(PropertyInfo pi, JsonElement parentNode, ref JsonElement result)
        {
            foreach (var node in parentNode.EnumerateObject())
            {
                JsonElement doc = default;
                if (node.Name == pi.Name && pi.PropertyType == typeof(T) && result.Equals(doc))
                {

                    result = node.Value;


                }
            }
        }

        private void DeserializeRecursive(PropertyInfo pi, object parent, JsonElement parentNode)
        {
            foreach (JsonProperty node in parentNode.EnumerateObject())
            {
                if (node.Name == pi.Name)
                {
                    if (pi.PropertyType == typeof(string))
                    {


                        pi.SetValue(parent, Convert.ChangeType(node.Value.ToString(), pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsPrimitive)
                    {
                        // Console.WriteLine($"{  pi.PropertyType}");
                        pi.SetValue(parent, Convert.ChangeType(node.Value.ToString(), pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsEnum)
                    {

                        pi.SetValue(parent, Enum.Parse(pi.PropertyType, node.ToString()));
                    }
                    else
                    {
                        Type subType = pi.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        pi.SetValue(parent, subObj);

                        PropertyInfo[] props = subType.GetProperties();
                        foreach (PropertyInfo ppi in props)
                        {
                            DeserializeRecursive(ppi, subObj, node.Value);
                        }
                    }
                }
            }
        }


    }
    //xml parser
    class XmlParser : IParser
    {
        private string path;

        private Type ttype;

        public XmlParser(string path, Type ttype)
        {
            this.path = path;
            this.ttype = ttype;
        }

        public T GetOptions<T>()
        {
            object result = Activator.CreateInstance(typeof(T));


            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo pi in properties)
                {
                    DeserializeRecursive(pi, result, FindNode<T>());
                }
            }
            catch (Exception)
            {

                result = null;
            }


            return (T)result;
        }

        private void DeserializeRecursive(PropertyInfo pi, object parent, XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name)
                {
                    if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
                    {
                        pi.SetValue(parent, Convert.ChangeType(node.InnerText, pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        pi.SetValue(parent, Enum.Parse(pi.PropertyType, node.InnerText));
                    }
                    else
                    {
                        Type subType = pi.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        pi.SetValue(parent, subObj);

                        PropertyInfo[] subPIs = subType.GetProperties();
                        foreach (PropertyInfo spi in subPIs)
                        {
                            DeserializeRecursive(spi, subObj, node);
                        }
                    }
                }
            }
        }

        private XmlNode FindNode<T>()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            if (typeof(T) == ttype)
            {
                return doc.DocumentElement;
            }

            PropertyInfo[] properties = ttype.GetProperties();

            XmlNode result = null;

            foreach (PropertyInfo ppi in properties)
            {
                FindNodeRecursive<T>(ppi, doc.DocumentElement, ref result);
            }

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }

        private void FindNodeRecursive<T>(PropertyInfo pi, XmlNode parentNode, ref XmlNode result)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name && pi.PropertyType == typeof(T) && result == null)
                {
                    result = node;

                    if (!pi.PropertyType.IsPrimitive && !(pi.PropertyType == typeof(string)))
                    {
                        Type subt = pi.PropertyType;

                        PropertyInfo[] props = subt.GetProperties();
                        foreach (PropertyInfo ppi in props)
                        {
                            FindNodeRecursive<T>(ppi, node, ref result);
                        }
                    }
                }
            }
        }
    }
}
