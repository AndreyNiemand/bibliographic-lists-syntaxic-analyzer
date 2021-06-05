using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class IntratextRef : Ref
    {
        public IntratextRef(string[] authors, string title, int? year, (uint?, uint?, uint?) pagesInfo, uint? tom, string publisher)
            : base(authors, title, year, pagesInfo, tom, publisher)
        {

        }
    }
}
