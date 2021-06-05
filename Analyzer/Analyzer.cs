
using CommandLine;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class Analyzer
    {
        public Standard Standard { get; set; } = null;

        public delegate void WriteFunction(string s);
        public WriteFunction Log { get; set; } = null;

        public Analyzer() {}

        public static List<string> ReadRefsFromDocx(string file)
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

        public void Analyze(string reference)
        {
            if (!Ref.TryParse(reference, out Ref r))
            {
                return;
            }

            Log(string.Format("Analysis: {0}", reference));

            var mistakes = Standard._Check(r);

            if (mistakes.Count != 0)
            {
                Log("Mistake(s): ");
                for (var i = 0; i < mistakes.Count; ++i)
                {
                    Log(string.Format(" {0}) {1}", i + 1, mistakes[i]));
                }
                Log("");
            }

            Log(string.Format("Right reference: {0}", Standard.GetRightRef()));
            Log("--------------------------------------------------------------");
        }
    }
}
