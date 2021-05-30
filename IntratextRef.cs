using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{
    class IntratextRef : IRef
    {
        public string[] Autors { get; }
        public int Year { get; }

        public (int, int?) Page { get; }

        public IntratextRef(string[] authors, int year, (int, int?) page)
        {
            this.Autors = authors;
            this.Year = year;
            this.Page = page;
        }
    }
}
