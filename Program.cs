
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    class Program
    { 
        static void Main(string[] args)
        {
            string data_in = "(Ахутин А.Б. Античные начала философии. СПб.: Наука, С.-Петерб. изд. фирма, 2007, С. 123-125)";

            var ref_ = IRef.Parse(data_in);


           
            Console.WriteLine("Hello World!");
        }
    }
}
