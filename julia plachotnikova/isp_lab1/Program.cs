using System;
using System.Collections;


namespace lab1
{
    static class Program
    {
        private static void Functions()
        {
            Console.WriteLine("1.Read from text file.");
            Console.WriteLine("2.Write to text file.");
            Console.WriteLine("3.Read from binary file.");
            Console.WriteLine("4.Write to binary file.");
            Console.WriteLine("5.Compress file.");
            Console.WriteLine("6.Decompress file.");
            Console.WriteLine("7.Rename file.");
            Console.WriteLine("8.Copy file.");
            Console.WriteLine("9.Delete file.");
            Console.WriteLine("10.End program.");
        }

        static string NameOfFile()
        {
            string name = Console.ReadLine();
            if (name != null && name.IndexOf('.') > 0)
            {
                name = name.Substring(0, name.IndexOf('.'));
            }

            return name;
        }

        private static void Main()
        {
            var folder = @"G:\JetBrains Rider 2019.3.3\USP(3sem)\";
            var list = new ArrayList();
            var file = new MyFile();
            Functions();
            var p = true;
            while (p)
            {
                Console.WriteLine("Enter number of function");
                int func;
                while (!int.TryParse(Console.ReadLine(), out func)) Console.WriteLine("Error. Enter one more time.");
                string doc;
                string zip;
                switch (func)
                {
                    case 1:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        file.Read(folder + doc, list);
                        break;
                    case 2:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        file.Write(folder + doc, list);
                        break;
                    case 3:
                        Console.WriteLine("Enter name of binary file");
                        doc = NameOfFile() + ".txt";
                        file.BinRead(folder + doc, list);
                        break;
                    case 4:
                        Console.WriteLine("Enter name of binary file");
                        doc = NameOfFile() + ".txt";
                        file.BinWrite(folder + doc, list);
                        break;
                    case 5:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        Console.WriteLine("Enter name of zip file");
                        zip = NameOfFile() + ".gz";
                        file.Compress(folder + doc, folder + zip);
                        break;
                    case 6:
                        Console.WriteLine("Enter name of zip file");
                        zip = NameOfFile() + ".gz";
                        Console.WriteLine("Enter name of recovered file");
                        doc = NameOfFile() + ".txt";
                        file.Decompress(folder + zip, folder + doc);
                        break;
                    case 7:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        Console.WriteLine("Enter new name of text file");
                        string doc2 = NameOfFile() + ".txt";
                        file.Rename(folder, ref doc, doc2);
                        break;
                    case 8:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        Console.WriteLine("Enter name of folder");
                        string folder2 = Console.ReadLine();
                        folder2 = folder + @"\" + folder2?.Trim(Convert.ToChar(@"\"), Convert.ToChar(@"."), Convert.ToChar(@",")) + @"\";
                        file.CopyFile(folder + doc, folder2, doc);
                        break;
                    case 9:
                        Console.WriteLine("Enter name of text file");
                        doc = NameOfFile() + ".txt";
                        file.DeleteFile(folder + doc);
                        break;
                    case 10:
                        p = false;
                        break;
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
            
        }
    }
}