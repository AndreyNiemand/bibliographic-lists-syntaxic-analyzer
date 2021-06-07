
using DocumentFormat.OpenXml.Packaging;
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

        public List<Ref> ReadRefsFromDocx(string file)
        {
            using (var doc = WordprocessingDocument.Open(file, false))
            {
                string docText = "";
                using (StreamReader sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                var refs = new List<Ref>();
                var parser = new Parser(Standard);

                var (reRB, reTB) = (new Regex(@"\(.*?\)"), new Regex(@"<.*?>"));
                for (var m = reRB.Match(docText); m.Success; m = m.NextMatch())
                {
                    var maybeIntratext = reTB.Replace(m.Value, "");
                    if (parser.TryParse(maybeIntratext, out IntratextRef intratext))
                    {
                        int j = 0;
                        for (; j < refs.Count; ++j)
                        {
                            if (Standard.IsRepeatRef(refs[j], intratext))
                            {
                                var r = refs[j] as RepeatIntratextRef != null 
                                    ? refs[j] as RepeatIntratextRef 
                                    : new RepeatIntratextRef(refs[j]);
                                r.ChildReferences.Add(intratext);
                                refs[j] = r;
                                break;
                            }
                        }

                        if (j == refs.Count)
                        { 
                            refs.Add(intratext); 
                        }
                        
                    }
                }
                
                if (doc.MainDocumentPart?.FootnotesPart?.Footnotes?.ChildElements != null)
                {
                    foreach (var subscript in doc.MainDocumentPart.FootnotesPart.Footnotes.ChildElements)
                    {
                        if (!string.IsNullOrEmpty(subscript.InnerText))
                        {
                            refs.Add(parser.Parse<SubscriptRef>(subscript.InnerText));
                        }
                    }
                }

                var (reRS, fmt) = (new Regex(@"\[(.*?)\]"), new Regex(@"(?<n>\d+,)?(?<r>.*)"));
                var rawOfftexts = new List<List<string>>();
               
                for (var m = reRS.Match(docText); m.Success; m = m.NextMatch())
                {
                    var maybeOfftext = reTB.Replace(m.Groups[1].Value, "");
                    var temp = new List<string>();
                    foreach (var t in maybeOfftext.Split(","))
                    {
                        temp.Add(t.Trim());
                    }
                    rawOfftexts.Add(temp);
                }

                var items = doc.MainDocumentPart.Document.ChildElements[0].ChildElements;
                int i = 0;
                for (; i < items?.Count; ++i)
                {
                    if (items[i]?.InnerText == "Ссылки")
                    {
                        ++i;
                        break;
                    }
                }

                for (; i < items?.Count; ++i)
                {
                    var temp = items[i]?.InnerText;
                    if (!string.IsNullOrEmpty(temp))
                    {
                        refs.Add(parser.Parse<OfftextRef>(temp));
                    }
                }

                return refs;
            }
        }

        public void Analyze(Ref r)
        {
            //Log(string.Format("Analysis: {0}", reference));

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

        public void Analyze(string reference)
        {
            var parser = new Parser(Standard);
            if (!parser.TryParse(reference, out IntratextRef r))
            {
                return;
            }

            Analyze(r);  
        }
    }
}
