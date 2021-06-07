
using System;
using System.Collections.Generic;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class Parser
    {
        Standard Standard { get; }

        public Parser(Standard std)
        {
            Standard = std;
        }

        public T Parse<T>(string s)
            where T : Ref, new()
        {
            var t = new T();

            t.Init(
                Authors: ParseAuthorsInfo(ref s), 
                Year: ParseYearInfo(ref s), 
                Pages: ParsePagesInfo(ref s),
                PageCount: ParsePagesCountInfo(ref s),
                Tom: ParseTomInfo(ref s),
                Title: ParseTitleInfo(ref s),
                Publisher: ParsePublisherInfo(ref s)
            );

            return t;
        }

        public virtual bool TryParse<T>(string s, out T r)
            where T : Ref, new()
        {
            r = Parse<T>(s);
            return Standard.EnoughInfoToBeParsed(r);
        }


        protected string ParsePublisherInfo(ref string s)
        {
            var regex = Standard.PublisherRegex;
            string p = null;

            var match = regex.Match(s);

            if (match.Success)
            {
                p = match.Value;
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return p;
        }
        protected string ParseTitleInfo(ref string s)
        {
            var regex = Standard.TitleRegex;

            string title = null;

            var match = regex.Match(s);

            if (match.Success)
            {
                title = match.Value;
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return title;
        }

        protected uint? ParseTomInfo(ref string s)
        {
            var regex = Standard.TomRegex;

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

        protected string[] ParseAuthorsInfo(ref string s)
        {
            var regex = Standard.AuthorsRegex;

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

        protected int? ParseYearInfo(ref string s)
        {
            var regex = Standard.YearRegex;

            int? year = null;
            var match = regex.Match(s);

            if (match.Success)
            {
                year = Int32.Parse(match.Value);
                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return year;
        }

        protected (uint?, uint?) ParsePagesInfo(ref string s)
        {
            var regex = Standard.PagesRegex;

            uint? page1 = null;
            uint? page2 = null;
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

                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return (page1, page2);
        }

        protected uint? ParsePagesCountInfo(ref string s)
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

                s = s.Remove(s.IndexOf(match.Value), match.Value.Length);
            }

            return count;
        }

    }
}
