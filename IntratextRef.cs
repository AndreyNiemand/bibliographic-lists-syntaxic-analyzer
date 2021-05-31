using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class IntratextRef : IRef
    {
        public string[] Autors { get; }
        public string? Title { get; }
        public int? Year { get; }
        public (int?, int?) Pages { get; }
        public uint? PageCount { get; }
        public uint? Tom { get; }
        public string? Publisher { get; }

        public IntratextRef(string[] authors, string? title, int? year, (int?, int?, uint?) pagesInfo, uint? tom, string? publisher)
        {
            this.Autors = authors;
            this.Title = title;
            this.Year = year;
            this.Pages = (pagesInfo.Item1, pagesInfo.Item2);
            this.PageCount = pagesInfo.Item3;
            this.Tom = tom;
            this.Publisher = publisher;
        }
    }
}
