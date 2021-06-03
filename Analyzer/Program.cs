
using CommandLine;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bibliographic_lists_syntaxic_analyzer
{
    class Program
    {
        static List<string> ReadRefsFromDocx(string file)
        {
            using (var doc = WordprocessingDocument.Open(file, false))
            {
                string docText = "";
                using (StreamReader sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                var refs = new List<string>();
                var (reRB, reTB) = (new Regex(@"\(.*?\)"), new Regex(@"<.*?>"));
                for (var m = reRB.Match(docText); m.Success; m = m.NextMatch())
                {
                    refs.Add(reTB.Replace(m.Value, ""));
                }

                return refs;
            }
        }

        delegate void WriteLineFunc(string s);

        static void Analyze(Standard standard, string reference, WriteLineFunc WriteLine)
        {
            if (!Ref.TryParse(reference, out Ref r))
            {
                return;
            }

            WriteLine(string.Format("Analysis: {0}", reference));

            var mistakes = standard.Check(r);

            if (mistakes.Count != 0)
            {
                WriteLine("Mistake(s): ");
                for (var i = 0; i < mistakes.Count; ++i)
                {
                    WriteLine(string.Format(" {0}) {1}", i + 1, mistakes[i]));
                }
                WriteLine("");
            }

            WriteLine(string.Format("Right reference: {0}", standard.GetRightRef()));
            WriteLine("--------------------------------------------------------------");
        }

        static void Main(string[] args)
        {
            args = @"-o a.txt -f C:\Users\user\Documents\bibliographic-lists-syntaxic-analyzer\Tests\IntratextRefTest.docx".Split(" ");

            CommandLineOptions options = null;
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(opt => options = opt)
                .WithNotParsed(errs =>
                {
                    foreach (var err in errs)
                    {
                        Console.WriteLine(err.ToString());
                    }
                });

            if (options != null)
            {
                StreamWriter writer = null;
                WriteLineFunc writeLine;

                if (options.Output == "console")
                {
                    writeLine = s => { Console.WriteLine(s); };
                }
                else
                {
                    writer = new StreamWriter(options.Output);
                    writer.AutoFlush = true;
                    writeLine = s => { writer.WriteLine(s); };
                }

                Standard standard = null;
                if (options.StandardName != null)
                {
                    try
                    {
                        standard = Standard.Get(options.StandardName);
                    }
                    catch (UnknownStandardException ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return;
                    }
                }

                if (options.SingleReference != null)
                {
                    Analyze(standard, options.SingleReference, writeLine);
                }
                
                if (options.File != null)
                {
                    var refs = ReadRefsFromDocx(options.File);
                    foreach (var r in refs)
                    {
                        Analyze(standard, r, writeLine);
                    }
                }

                writer?.Dispose();
            }
        }
    }
}
