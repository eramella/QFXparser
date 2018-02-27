using Newtonsoft.Json;
using QFXparser;
using System;
using System.IO;

namespace QFXparser.TestApp {
    class Program {
        static void Main (string[] args) {
            Console.Write("Type the path of the file you would like to upload: ");
            string qfxpath = Console.ReadLine();//Directory.GetParent("ofx.qbo").Parent.FullName + "\\Files\\ofx.qbo";
            Stream stream = new FileStream(qfxpath, FileMode.Open);
            FileParser parser = new FileParser(stream);

            Console.WriteLine("Starting to parse...");
            Statement result = parser.BuildStatement();
            var str = JsonConvert.SerializeObject(result);
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}