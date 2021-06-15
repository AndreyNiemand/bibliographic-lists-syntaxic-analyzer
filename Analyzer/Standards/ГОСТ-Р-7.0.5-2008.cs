
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class ГОСТ_Р_705_2008 : Standard
    {
        public override Regex PublisherRegex => new Regex(@"(?<p>[\w-]+\s*\.?\s*(:[\w\s]+)?)");
        public override Regex TitleRegex => new Regex(@"(?<t>\w[\w\s,:]*(\.{1,3}|…))");
        public override Regex TomRegex => new Regex(@"[TtТт]\.\s*(?<t>\d+)");
        public override Regex AuthorsRegex => new Regex(@"(?<a>\w+\s+\w\.\s*\w\.)");
        public override Regex YearRegex => new Regex(@"(?<y>\d{4})");
        public override Regex PagesRegex => new Regex(@"[cCсС]\. (?<p1>\d+)(\p{Pd}(?<p2>\d+))?");
        public override Regex PagesCountRegex => new Regex(@"(?<c>\d+) с\.");
        public override string ComplexSeparator => " ; ";
        public override string Ibid => "Там же";
        public override string OfftextRefNameOfHeading => "Ссылки";
        protected override (string,string) IntratextRefCharsAround => ("(", ")");
        protected override (string,string) OfftextPartRefCharsAround => ("[", "]");

        public override bool IsRepeatRef(Ref first, Ref repeat)
        {
            if (first == null || repeat == null)
            {
                return false;
            }

            var authorsEquality = true;
            for (int i = 0; authorsEquality && i < first.Authors?.Length && i < repeat.Authors?.Length; ++i)
            {
                authorsEquality = authorsEquality && (first.Authors[i] == repeat.Authors[i]);
            }

            var (t1, t2) = (first.Title, repeat.Title);

            if (t2 != null && (t2.EndsWith("...") || t2.EndsWith("…")))
            {
                t1 = t1.Substring(0, t2.Length-5);
                t2 = t2.Substring(0, t2.Length-5);
            }

            return authorsEquality && (t1 == t2);
        }
        public override bool IsPartOfOfftextRef(Ref part, Ref r)
        {
            if (part == null || r == null)
            {
                return false;
            }

            var authorsEquality = true;
            for (int i = 0; authorsEquality && i < part.Authors?.Length && i < r.Authors?.Length; ++i)
            {
                authorsEquality = authorsEquality && (part.Authors[i] == r.Authors[i]);
            }

            return authorsEquality;
        }

        protected override void Check(Ref r)
        {
            if (r.Type == Ref.PositionType.OfftextPart)
                if(r.FirstRef == null) 
                    Log("Ссылка не связана ни с одной затекстовой ссылкой.");

            if (r.Pages.Item1 != null && r.Pages.Item2 != null)
                if (r.Pages.Item1 >= r.Pages.Item2)
                {
                    Log(string.Format("Номер первой страницы ({0}) должен быть меньше номера второй страницы ({1}).", r.Pages.Item1, r.Pages.Item2));
                    Pattern(r.Pages, "?");
                }

            if (r.Pages.Item2 != null && r.PageCount != null)
                if (r.Pages.Item2 > r.PageCount)
                {
                    Log(string.Format("Номер второй страницы ({0}) должен быть меньше или равен объему ({1}) источника.", r.Pages.Item2, r.PageCount));
                    Pattern(r.PageCount, "?");
                }

            if ((r.PageCount == null) == (r.Pages.Item1 == null))
            {
                Log("В источнике не могут одновременно отсутствовать объем " +
                "и страницы, на которые он ссылается. " +
                "Добавьте номера страниц, например: \"С. 123\" или " +
                "\"С. 123-124\", или общее их количество: \"500 c.\"");
            }

            if (r.FirstRef != null)
            {
                if (r.Pages.Item1 != null && r.FirstRef.PageCount != null)
                    if (r.Pages.Item1 > r.FirstRef.PageCount)
                        Log(string.Format("Страница ({0}), на которую указывает ссылка, должна входить в диапазон: [1 .. {1}].",
                            r.Pages.Item1, r.FirstRef.PageCount));

                if (r.FirstRef.Pages.Item1 != null && r.PageCount != null)
                    if (r.FirstRef.Pages.Item1 > r.PageCount)
                        Log(string.Format("Количество страниц ({0}) в повторной ссылке должно быть больше, чем первичной ({1}).",
                            r.PageCount, r.FirstRef.Pages.Item1));

               // ...
            }

            DefaultSeparator = " ";
            Separate(r.Authors, ", ");
            Separate(r.Year, ". ", r.Pages);
            Separate(r.Year, ". ", r.PageCount);
            Separate(r.Pages, "—");
            Separate(r.Publisher, ", ", r.Year);

            Pattern("С. ", r.Pages);
            Pattern(r.PageCount, " с.");
            Pattern("Т. ", r.Tom);

            if (r.Authors == null || r.Authors.Length < 4)
            {
                Order(r.Authors, 0);
                Order(r.Title, 1);
            }
            else
            {
                Order(r.Title, 0);
                Order(r.Authors, 1);
                Separate(r.Title, " / ", r.Authors);
            }
        }
    }
}
