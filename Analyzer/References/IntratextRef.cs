using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class IntratextRef : Ref
    {

        public IntratextRef() 
        { 
        
        }


        public IntratextRef(string[] Authors, int? Year, (uint?, uint?) Pages, uint? PageCount, uint? Tom, string Title, string Publisher)
        {
            this.Authors = Authors;
            this.Year = Year;
            this.Pages = Pages;
            this.PageCount = PageCount;
            this.Tom = Tom;
            this.Title = Title;
            this.Publisher = Publisher;
        }
    }
}
