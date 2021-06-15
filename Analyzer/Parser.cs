
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class Parser
    {
        Standard Standard { get; }

        public Parser(Standard std)
        {
            Standard = std;
        }

        public Ref Parse(string s, Ref.PositionType type, List<Ref> complexRelationship = null)
        { 
            var d = s.Length;

            var t = new Ref(
                Raw: s,
                Type: type,
                ComplexRelationship: complexRelationship,

                Authors:    ParseAuthorsInfo    (ref s, out int authorsIndex), 
                Year:       ParseYearInfo       (ref s, out int yearIndex), 
                Pages:      ParsePagesInfo      (ref s, out int pagesIndex),
                PageCount:  ParsePagesCountInfo (ref s, out int pagesCountIndex),
                Tom:        ParseTomInfo        (ref s, out int tomIndex),
                Title:      ParseTitleInfo      (ref s, out int titleIndex),
                Publisher:  ParsePublisherInfo  (ref s, out int publisherIndex)
            );

            var order =  new Dictionary<int, object>()
            {
                [authorsIndex] = t.Authors,
                [yearIndex] = t.Year,
                [pagesIndex] = t.Pages,
                [pagesCountIndex] = t.PageCount,
                [tomIndex] = t.Tom,
                [titleIndex] = t.Title,
                [publisherIndex] = t.Publisher,
            }.OrderBy(pair => pair.Key); 

            return t;
        }

        public bool TryParse(string s, out Ref r, Ref.PositionType posType)
        {
            if (s.Contains(Standard.ComplexSeparator))
            {
                var rawRefs = s.Split(Standard.ComplexSeparator);
                var refs = new List<Ref>(rawRefs.Length);

                for(var i = 0; i < rawRefs.Length; ++i)
                {
                    refs.Add(Parse(rawRefs[i], posType, refs));
                }

                r = refs[0];
            }
            else
            {
                r = Parse(s, posType);
            }

            return Standard.EnoughInfoToBeParsed(r);
        }

        protected string ParsePublisherInfo(ref string s, out int publisherIndex)
        {
            var regex = Standard.PublisherRegex;
            string p = null;

            var match = regex.Match(s);

            if (match.Success)
            {
                p = match.Value;
                publisherIndex = s.IndexOf(match.Value);
                s = s.Remove(publisherIndex, match.Value.Length);
            }
            else
            {
                publisherIndex = -1;
            }

            return p;
        }
        protected string ParseTitleInfo(ref string s, out int titleIndex)
        {
            var regex = Standard.TitleRegex;

            string title = null;

            var match = regex.Match(s);

            if (match.Success)
            {
                title = match.Value;
                titleIndex = s.IndexOf(match.Value);
                s = s.Remove(titleIndex, match.Value.Length);
            }
            else
            {
                titleIndex = -1;
            }

            return title;
        }

        protected uint? ParseTomInfo(ref string s, out int tomIndex)
        {
            var regex = Standard.TomRegex;

            uint? tom = null;
            var match = regex.Match(s);

            tomIndex = -1;

            if (match.Success)
            {
                var t = match.Groups["t"];
                if (t.Success)
                {
                    tom = UInt32.Parse(t.Value);
                    tomIndex = s.IndexOf(match.Value);
                    s = s.Remove(tomIndex, match.Value.Length);
                }
                
            }
            
            return tom;
        }

        protected string[] ParseAuthorsInfo(ref string s, out int authorsIndex)
        {
            var regex = Standard.AuthorsRegex;

            List<string> authors = new List<string>();

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            {
                authors.Add(match.Value);
            }

            if (authors.Count != 0)
            {
                for (int i = 0; i < authors.Count; ++i)
                {
                    var begin = s.IndexOf(authors[i]);
                    var end = authors[i].Length - 1;

                    s = s.Remove(begin, end);
                }
   
                authorsIndex = s.IndexOf(authors[0]);

                //var end = s.IndexOf(authors[authors.Count - 1]) - s.IndexOf(authors[0]) + authors[authors.Count - 1].Length + 1;
                //s = s.Remove(authorsIndex, end);
            }
            else
            {
                authorsIndex = -1;
            }

            return authors.Count != 0 ? authors.ToArray() : null;
        }

        protected int? ParseYearInfo(ref string s, out int yearIndex)
        {
            var regex = Standard.YearRegex;

            int? year = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                year = Int32.Parse(match.Value);
                yearIndex = s.IndexOf(match.Value);
                s = s.Remove(yearIndex, match.Value.Length);
            }
            else
            {
                yearIndex = -1;
            }

            return year;
        }

        protected (uint?, uint?) ParsePagesInfo(ref string s, out int pagesIndex)
        {
            var regex = Standard.PagesRegex;

            uint? page1 = null;
            uint? page2 = null;
            var match = regex.Match(s);
            pagesIndex = -1;

            if (match.Success)
            {
                var p1 = match.Groups["p1"];
                if (p1.Success)
                {
                    page1 = UInt32.Parse(p1.Value);
                }

                var p2 = match.Groups["p2"];
                if (p2.Success)
                {
                    page2 = UInt32.Parse(p2.Value);
                }
                pagesIndex = s.IndexOf(match.Value);
                s = s.Remove(pagesIndex, match.Value.Length);
            }

            return (page1, page2);
        }

        protected uint? ParsePagesCountInfo(ref string s, out int pagesCountIndex)
        {
            var regex = Standard.PagesCountRegex;

            uint? count = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                var c = match.Groups["c"];
                if (c.Success)
                {
                    count = UInt32.Parse(c.Value);
                }

                pagesCountIndex = s.IndexOf(match.Value);
                s = s.Remove(pagesCountIndex, match.Value.Length);
            }
            else 
            {
                pagesCountIndex = -1;
            }

            return count;
        }

        public List<Ref> ParseRefsFromDocx(string file)
        {
            using (var doc = WordprocessingDocument.Open(file, false))
            {
                var refs = ParseIntratextRefs(doc.MainDocumentPart);

                refs.AddRange(ParseSubscriptRefs(doc.MainDocumentPart?.FootnotesPart?.Footnotes?.ChildElements));

                refs.AddRange(ParseOfftextRefs(doc.MainDocumentPart));

                return refs;
            }
        }

        private List<Ref> ParseIntratextRefs(MainDocumentPart docPart)
        {
            var resultRefs = new List<Ref>();

            var reg = new Regex(@"\(.*?\)");

            for (var m = reg.Match(docPart.Document.InnerText); m.Success; m = m.NextMatch())
            {
                var maybeIntratext = m.Value;
                if (TryParse(maybeIntratext, out Ref intratext, Ref.PositionType.Intratext))
                {
                    bool repeatDetermined = false;

                    for (var j = resultRefs.Count() - 1; j >= 0; --j)
                    {
                        if (repeatDetermined) break;

                        var firsts = resultRefs[j].ComplexRelationship ?? new List<Ref>() { resultRefs[j] };
                        var repeats = intratext.ComplexRelationship ?? new List<Ref>() { intratext };

                        if (firsts != null && repeats != null)
                        {
                            foreach (var f in firsts)
                            {
                                foreach (var r in repeats)
                                {
                                    if (Standard.IsRepeatRef(f, r))
                                    {
                                        r.FirstRef = f;
                                        repeatDetermined = true;
                                    }
                                }
                            }
                        }
                    }

                    resultRefs.Add(intratext);
                }
            }

            return resultRefs;
        }

        private List<Ref> ParseSubscriptRefs(OpenXmlElementList list)
        {
            var resultRefs = new List<Ref>();

            foreach (var subscript in list)
            {
                if (!string.IsNullOrEmpty(subscript.InnerText))
                {
                    var ibidIndex = subscript.InnerText.IndexOf(Standard.Ibid);
                    var raw = subscript.InnerText;
                    Ref r = null;

                    if (ibidIndex != -1)
                    {
                        raw = raw.Substring(ibidIndex, Standard.Ibid.Length);
                        r = Parse(raw, Ref.PositionType.Subscript);
                        for (var j = resultRefs.Count() - 1; j >= 0; --j)
                        {
                            if (resultRefs[j].FirstRef != null)
                            {
                                r.FirstRef = resultRefs[j];
                                break;
                            }
                        }
                    }
                    else
                    {
                        r = Parse(raw, Ref.PositionType.Subscript);
                    }

                    resultRefs.Add(r);
                }
            }

            return resultRefs;
        }

        private List<Ref> ParseOfftextRefs(MainDocumentPart docPart)
        {
            var resultRefs = new List<Ref>();

            var reg = new Regex(@"\[\s*((?<n>\d+?),)?(?<r>.*?)\s*\]");
            var OfftextsParts = new Dictionary<int, Ref>() { [-1] = null };

            for (var m = reg.Match(docPart.Document.InnerText); m.Success; m = m.NextMatch())
            {
                var n = m.Groups["n"].Success
                    ? Int32.Parse(m.Groups["n"].ToString())
                    : OfftextsParts.Keys.Min() - 1;

                var r = m.Groups["r"].ToString();

                OfftextsParts.Add(n, Parse(r, Ref.PositionType.OfftextPart));
            }

            var rawRefs = docPart.Document.ChildElements[0].ChildElements;

            var i = rawRefs.Count() - 1;
            for (; i >= 0; --i)
            {
                if (rawRefs[i].InnerText == Standard.OfftextRefNameOfHeading)
                    break;
            }

            for (++i; i < rawRefs.Count(); ++i)
            {
                var text = rawRefs.ElementAt(i).InnerText;
                if (string.IsNullOrEmpty(text))
                {
                    break;
                }

                var r = Parse(text, Ref.PositionType.Offtext);
                try
                {
                    OfftextsParts[i].FirstRef = r;
                }
                catch (KeyNotFoundException)
                {
                    var others = OfftextsParts.Where(x => x.Key < -1);
                    foreach (var (_, part) in others)
                    {
                        if (Standard.IsPartOfOfftextRef(part, r))
                        {
                            part.FirstRef = r;
                            break;
                        }
                    }
                }

                for (var j = resultRefs.Count() - 1; j >= 0; --j)
                {
                    if (Standard.IsRepeatRef(r, resultRefs[j]))
                    {
                        r.FirstRef = resultRefs[j];
                        break;
                    }
                }

                resultRefs.Add(r);
            }

            OfftextsParts.Remove(-1);
            resultRefs.AddRange(OfftextsParts.Values);

            return resultRefs;
        }
    }
}
