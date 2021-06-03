using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public abstract class Ref
    {
        public string[] Autors { get; }
        public int? Year { get; }
        public (uint?, uint?) Pages { get; }
        public uint? PageCount { get; }
        public uint? Tom { get; }
        public string Title { get; }
        public string Publisher { get; }

        public Ref(string[] authors, string title, int? year, (uint?, uint?, uint?) pagesInfo, uint? tom, string publisher)
        {
            this.Autors = authors;
            this.Title = title;
            this.Year = year;
            this.Pages = (pagesInfo.Item1, pagesInfo.Item2);
            this.PageCount = pagesInfo.Item3;
            this.Tom = tom;
            this.Publisher = publisher;
        }

        public static Ref Parse(string s)
        {
            return new IntratextRef(
                authors: ParseAuthorsInfo(ref s),
                year: ParseYearInfo(ref s),
                pagesInfo: ParsePagesInfo(ref s),
                tom: ParseTomInfo(ref s),
                title: ParseTitleInfo(ref s),
                publisher: ParsePublisherInfo(ref s)
            );
        }

        public static bool TryParse(string s, out Ref r)
        {
            r = Parse(s);
            return (r.Autors != null || r.Year != null) && r.Title != null;
        }

        private static string ParsePublisherInfo(ref string s)
        {
            var regex = new Regex(@"[\w-]+\s*\.?\s*(:[\w\s]+)?");
            string p = null;

            var match = regex.Match(s);

            if(match.Success)
            {
                p = match.Value;
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return p;
        }
        private static string ParseTitleInfo(ref string s)
        {
            var regex = new Regex(@"\w[\w\s,:]*\.");

            string title = null;

            var match = regex.Match(s);

            if (match.Success)
            { 
                title = match.Value;
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return title;
        }

        private static uint? ParseTomInfo(ref string s)
        {
            var regex = new Regex(@"[TtТт]\.\s*(?<t>\d+)");

            uint? tom = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                var t = match.Groups["t"];
                if (t.Success)
                {
                    tom = UInt32.Parse(t.Value);
                    s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
                }
            }

            return tom;
        }

        private static string[] ParseAuthorsInfo(ref string s)
        {
            var regex = new Regex(@"\w+\s+\w\.\s*\w\.");

            List<string> authors = new List<string>();

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            {
                authors.Add(match.Value);
            }

            if (authors.Count != 0) 
            {
                var begin = s.IndexOf(authors[0]);
                var end = s.IndexOf(authors[authors.Count - 1]) - s.IndexOf(authors[0]) + authors[authors.Count - 1].Length + 1;
                s = s.Remove(begin, end);
            }

            return authors.Count != 0 ? authors.ToArray() : null;
        }

        private static int? ParseYearInfo(ref string s)
        {
            Regex regex = new Regex(@"\d{4}");

            int? year = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                year = Int32.Parse(match.Value);
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return year;
        }

        private static (uint?, uint?, uint?) ParsePagesInfo(ref string s)
        {
            Regex regex = new Regex(@"((?<c>\d+) с\.)|(С\. (?<p1>\d+)(\p{Pd}(?<p2>\d+))?)");

            uint? page1 = null;
            uint? page2 = null;
            uint? count = null;
            var match = regex.Match(s);

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

                var c = match.Groups["c"];
                if (c.Success)
                {
                    count = UInt32.Parse(c.Value);
                }

                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return (page1, page2, count);
        }
    }
}
