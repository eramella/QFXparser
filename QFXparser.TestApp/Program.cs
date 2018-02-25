using Newtonsoft.Json;
using QFXparser.Core;
using System;
using System.IO;

namespace QFXparser.TestApp {
    class Program {
        static void Main (string[] args) {
            string qfxpath = Directory.GetParent("ofx.qbo").Parent.FullName + "\\Files\\ofx.qbo";
            Stream stream = new FileStream(qfxpath, FileMode.Open);   
            StatementBuilder builder = new StatementBuilder(stream);

            Console.WriteLine("Starting to parse...");
            var result = builder.Build();
            var str = JsonConvert.SerializeObject(result);
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}