
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Standard s = Standard.Get("ГОСТ-Р-7.0.5-2008");
                var mistakes = s.Check("(Мельников В.П., Клейменов С.А., Петраков A.M. Информационная безопасность и защита информации, учебное пособие. М., 2006, 123 с.)");
                var r = s.GetRightRef();
            }
            catch (UnknownStandardException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
