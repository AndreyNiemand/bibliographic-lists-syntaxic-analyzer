using System;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class ГОСТ_Р_705_2008 : Standard
    {
        protected override void _Check(Ref r)
        {
            Mustbe(r.Pages.Item1 != null && r.Pages.Item2 != null && r.Pages.Item1 < r.Pages.Item2,
                    string.Format("Номер первой страницы ({0}) должен быть меньше номера второй ({1}).", r.Pages.Item1, r.Pages.Item2));

            Mustbe(r.Pages.Item2 != null && r.PageCount != null && r.Pages.Item2 <= r.PageCount,
                   string.Format("Номер второй страницы ({0}) должен быть меньше или равен объему ({1}) источника.", r.Pages.Item2, r.PageCount));

            Mustbe((r.PageCount != null) != ((r.Pages.Item1 != null) && (r.Pages.Item2 != null)),
                "В источнике не могут одновременно отсутствовать объем и страницы, на которые он ссылается.");

            DefaultSeparator = " ";
            Separate(r.Autors, ", ");
            Separate(r.Pages, " - ", r.Year);
            Separate(r.Pages, "-");

            Pattern("С. ", r.Pages);
            Pattern(r.PageCount, " с.");
            Pattern("Т. ", r.Tom);

            if (r.Autors.Length < 3)
            {
                Order(r.Autors, 0);
                Order(r.Title, 1);
            }
            else
            {
                Order(r.Title, 0);
                Order(r.Autors, 1);
                Separate(r.Title, " // ", r.Autors);
            }
        }
    }
}
