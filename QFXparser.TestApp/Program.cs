using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace QFXparser.TestApp
{
    class Program {
        static void Main (string[] args) {
            Console.Write("Type the path of the file you would like to upload: ");
            string qfxpath = Console.ReadLine();//Directory.GetParent("ofx.qbo").Parent.FullName + "\\Files\\ofx.qbo";
            // Stream stream = new FileStream(qfxpath, FileMode.Open);
            // FileParser parser = new FileParser(stream);

            // Encoding encoding1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            var parser = new FileParser(qfxpath); // new StreamReader(qfxpath, encoding1252));


            Console.WriteLine("Starting to parse...");
            Statement result = parser.BuildStatement();
            var str = JsonConvert.SerializeObject(result);
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}