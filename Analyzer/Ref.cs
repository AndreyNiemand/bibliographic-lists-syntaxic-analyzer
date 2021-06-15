using System.Collections.Generic;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class Ref
    {
        public string Raw { get; protected set; }

        public string[] Authors { get; set; }
        public int? Year { get; set; }
        public (uint?, uint?) Pages { get; set; }
        public uint? PageCount { get; set; }
        public uint? Tom { get; set; }
        public uint? Number { get; set; }
        public uint? Part { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }   

        public enum PositionType { Intratext, Subscript, Offtext, OfftextPart }

        public PositionType Type { get; protected set; }
        public Ref FirstRef { get; set; }
        public List<Ref> ComplexRelationship { get; protected set; }

        public Ref(string Raw, PositionType Type, List<Ref> ComplexRelationship, string[] Authors, int? Year, (uint?, uint?) Pages, uint? PageCount, uint? Tom, string Title, string Publisher)
        {
            this.Raw = Raw;
            this.Type = Type;
            this.ComplexRelationship = ComplexRelationship;
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
