using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public interface IRef
    {
        public string[] Autors { get; }
        public int? Year { get; }
        public (int?, int?) Pages { get; }
        public uint? PageCount { get; }
        public uint? Tom { get; }
        public string? Title { get; }
        public string? Publisher { get; }

        public static IRef Parse(string s)
        {
            return new IntratextRef(
                authors: ParseAuthorsInfo(ref s),
                year: ParseYearInfo(ref s),
                pagesInfo: ParsePagesInfo(ref s),
                tom: ParseTomInfo(ref s),
                publisher: ParsePublisherInfo(s),
                title: ParseTitleInfo(s)
            );
        }

        private static string? ParsePublisherInfo(string s)
        {
            var regex = new Regex(@"([\w-]+\.?)(:[\w\s]+)?");

            string? p = null;
            List<string> list = new List<string>();

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            {
                p = match.Value;
                list.Add(p);
            }

            return p;
        }
        private static string? ParseTitleInfo(string s)
        {
            var regex = new Regex(@"\w[\w\s,:]*\.");

            string? title = null;

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            { 
                if (title == null || title.Length < match.Value.Length)
                {
                    title = match.Value;
                }
            }

            return title;
        }

        private static uint? ParseTomInfo(ref string s)
        {
            var regex = new Regex(@"T\. (?<t>\d+)");

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
            var regex = new Regex(@"\w+ \w\.\w\.");

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

        private static (int?, int?, uint?) ParsePagesInfo(ref string s)
        {
            Regex regex = new Regex(@"((?<c>\d+) с\.)|(С\. (?<p1>\d+)(-(?<p2>\d+))?)");

            int? page1 = null;
            int? page2 = null;
            uint? count = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                var p1 = match.Groups["p1"];
                if (p1.Success)
                {
                    page1 = Int32.Parse(p1.Value);
                }

                var p2 = match.Groups["p2"];
                if (p2.Success)
                {
                    page2 = Int32.Parse(p2.Value);
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
