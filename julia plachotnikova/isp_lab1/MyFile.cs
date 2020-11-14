using System;
using System.Collections;
using System.IO;
using System.IO.Compression;

namespace lab1
{
    public class MyFile
    {
        public void Read(string path, ArrayList list)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Write(string path, ArrayList list)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                {
                    foreach (var o in list)
                    {
                        sw.WriteLine(o);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void BinRead(string path, ArrayList list)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    // пока не достигнут конец файла
                    // считываем каждое значение из файла
                    while (reader.PeekChar() > -1)
                    {
                        list.Add(reader.ReadString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void BinWrite(string path, ArrayList list)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    foreach (var o in list)
                    {
                        writer.Write(o as string);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("MyFile compression {0} completed. Original size: {1}  Compressed size: {2}.",
                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }

        public void Decompress(string compressedFile, string targetFile)
        {
            FileInfo fileInfo = new FileInfo(compressedFile);
            if (fileInfo.Exists)
            {
                // поток для чтения из сжатого файла
                using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
                {
                    // поток для записи восстановленного файла
                    using (FileStream targetStream = File.Create(targetFile))
                    {
                        // поток разархивации
                        using (GZipStream decompressionStream =
                            new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                            Console.WriteLine("MyFile restored: {0}", targetFile);
                        }
                    }
                }
            }
            else Console.WriteLine("Error. Compressed file don't exist.");
        }

        public bool IsChanged(string path)
        {
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                var zero = TimeSpan.Zero;
                var ctime = fileInf.LastAccessTime.Subtract(fileInf.CreationTime);
                if (ctime > zero)
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteFile(string path)
        {
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
        }

        public void CopyFile(string path, string newPath, string name)
        {
            try
            {
                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                {
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    fileInf.CopyTo(newPath + name, true);
                }
                else Console.WriteLine("File with name {0} don't exist.", path);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void Rename(string folder, ref string docName, string newName)
        {
            FileInfo fileInf = new FileInfo(folder + docName);
            if (fileInf.Exists)
            {
                fileInf.MoveTo(folder + newName);
            }
            else Console.WriteLine("File with name {0} don't exist.", folder + docName);
        }
    }
}