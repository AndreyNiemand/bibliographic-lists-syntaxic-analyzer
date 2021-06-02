
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    class Program
    {
        static List<Ref> ReadRefsFromDocx(string file)
        {
            using (var doc = WordprocessingDocument.Open(file, false))
            {
                string docText = "";
                using (StreamReader sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                var refs = new List<Ref>();
                var (reRB, reTB) = (new Regex(@"\(.*?\)"), new Regex(@"<.*?>"));
                for (var m = reRB.Match(docText); m.Success; m = m.NextMatch())
                {
                    var t = reTB.Replace(m.Value, "");
                    if (Ref.TryParse(t, out Ref r))
                    {
                        refs.Add(r);
                    }
                }

                return refs;
            }
        }

        static void Main(string[] args)
        {
            var refs = ReadRefsFromDocx(@"..\..\..\..\Tests\IntratextRefTest.docx");

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
