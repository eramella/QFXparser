using Newtonsoft.Json;
using System;

namespace QFXparser.TestApp
{
    class Program {
        static void Main (string[] args) {
            Console.Write("Type the path of the file you would like to upload: ");
            string qfxpath = Console.ReadLine();
            var parser = new FileParser(qfxpath);

            Console.WriteLine("Starting to parse...");
            Statement result = parser.BuildStatement();
            var str = JsonConvert.SerializeObject(result);
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}