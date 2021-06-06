using bibliographic_lists_syntaxic_analyzer;

using NUnit.Framework;

namespace Tests
{
    public class IntratextRefTests
    {
        readonly Parser parser = new Parser(Standard.Get("ГОСТ-Р-7.0.5-2008"));

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            string data_in = "(Мельников В.П., " +
                "Клейменов С.А., Петраков A.M. " +
                "Информационная безопасность и защита информации, " +
                "учебное пособие. М., 2006)";
            var r = parser.Parse<IntratextRef>(data_in);

            Assert.AreEqual(3, r.Authors.Length);
            Assert.AreEqual("Мельников В.П.", r.Authors[0]);
            Assert.AreEqual("Клейменов С.А.", r.Authors[1]);
            Assert.AreEqual("Петраков A.M.", r.Authors[2]);

            Assert.AreEqual(2006, r.Year);

            Assert.AreEqual("М.", r.Publisher);
            Assert.AreEqual("Информационная безопасность и защита " +
                "информации, учебное пособие.", r.Title);

            Assert.AreEqual(null, r.Tom);
            Assert.AreEqual(null, r.Pages.Item1);
            Assert.AreEqual(null, r.Pages.Item2);
            Assert.AreEqual(null, r.PageCount);
        }

        [Test]
        public void Test2()
        {
            string data_in = "(Потемкин В.К., Казаков Д.Н. Социальное партнерство: формирование, оценка, регулирование. СПб., 2002, 202 с.)";
            var r = parser.Parse<IntratextRef>(data_in);

            Assert.AreEqual(2, r.Authors.Length, 2);
            Assert.AreEqual("Потемкин В.К.", r.Authors[0]);
            Assert.AreEqual("Казаков Д.Н.", r.Authors[1]);

            Assert.AreEqual("СПб.", r.Publisher);
            Assert.AreEqual("Социальное партнерство: формирование, оценка, регулирование.", r.Title);

            Assert.AreEqual(2002, r.Year);

            Assert.AreEqual(null, r.Tom);
            Assert.AreEqual(null, r.Pages.Item1);
            Assert.AreEqual(null, r.Pages.Item2);
            Assert.AreEqual(202, r.PageCount);
        }

        [Test]
        public void Test3()
        {
            string data_in = "(Аренс В.Ж. Азбука исследователя. М. : Интермет Инжиниринг, 2006)";
            var r = parser.Parse<IntratextRef>(data_in);

            Assert.AreEqual(1, r.Authors.Length);
            Assert.AreEqual("Аренс В.Ж.", r.Authors[0]);

            Assert.AreEqual("М. : Интермет Инжиниринг", r.Publisher);
            Assert.AreEqual("Азбука исследователя.", r.Title);

            Assert.AreEqual(2006, r.Year);

            Assert.AreEqual(null, r.Tom);
            Assert.AreEqual(null, r.Pages.Item1);
            Assert.AreEqual(null, r.Pages.Item2);
            Assert.AreEqual(null, r.PageCount);
        }

        //[Test]
        //public void Test4()
        //{
        //    var output = new List<string>();
        //    var analyzer = new Analyzer() { Standard = Standard.Get("ГОСТ-Р-7.0.5-2008"), Log = (s) => output.Add(s) };

        //    var refs = Analyzer.ReadRefsFromDocx("../../../IntratextRefTest.docx");
        //    foreach (var r in refs)
        //    {
        //        analyzer.Analyze(r);
        //    }
        //    // ...
        //}
    }
}