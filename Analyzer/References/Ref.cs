namespace bibliographic_lists_syntaxic_analyzer
{
    public abstract class Ref
    {
        public string[] Authors { get; protected set; }
        public int? Year { get; protected set; }
        public (uint?, uint?) Pages { get; protected set; }
        public uint? PageCount { get; protected set; }
        public uint? Tom { get; protected set; }
        public string Title { get; protected set; }
        public string Publisher { get; protected set; }

        public void Init(string[] Authors, int? Year, (uint?, uint?) Pages, uint? PageCount, uint? Tom, string Title, string Publisher)
        {
            this.Authors = Authors;
            this.Year = Year;
            this.Pages = Pages;
            this.PageCount = PageCount;
            this.Tom = Tom;
            this.Title = Title;
            this.Publisher = Publisher;
        }

        protected void Init(Ref r)
        {
            Init(r.Authors, r.Year, r.Pages, r.PageCount, r.Tom, r.Title, r.Publisher);
        }

        public Ref()
        { 
        
        }
    }
}
