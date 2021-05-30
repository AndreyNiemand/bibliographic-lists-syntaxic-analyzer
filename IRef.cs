using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    interface IRef
    {
        public string[] Autors { get; }
        public int Year { get; }
        public (int, int?) Page { get; }

        public static IRef Parse(string s)
        {
            return new IntratextRef(ParseAuthors(s), ParseYear(s), ParsePage(s));
        }

        private static string[] ParseAuthors(string s)
        {
            var regex = new Regex(@"\w+ \w\.\w\.");

            List<string> authors = new List<string>();

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            {
                authors.Add(match.Value);
            }

            return authors.ToArray();
        }

        private static int ParseYear(string s)
        {
            Regex regex = new Regex(@"\d{4}");

            int year = 0;

            for (var match = regex.Match(s); match.Success; match = match.NextMatch())
            {
                year = Int32.Parse(match.Value);
            }

            return year;
        }

        private static (int, int?) ParsePage(string s)
        {
            Regex regex = new Regex(@"((?<p1>\d+) с\.)|(С\. (?<p1>\d+)(-(?<p2>\d+))?)");

            int page1 = 0;
            int? page2 = null;

            for (Match match = regex.Match(s); match.Success; match = match.NextMatch())
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
            }

            return (page1, page2);
        }
    }
}
