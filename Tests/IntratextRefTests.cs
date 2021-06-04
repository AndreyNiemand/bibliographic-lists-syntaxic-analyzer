using NUnit.Framework;
using bibliographic_lists_syntaxic_analyzer;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Collections.Generic;

namespace Tests
{
    public class IntratextRefTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            string data_in = "(��������� �.�., ��������� �.�., �������� A.M. �������������� ������������ � ������ ����������, ������� �������. �., 2006)";
            var r = Ref.Parse(data_in);

            Assert.AreEqual(3, r.Autors.Length);
            Assert.AreEqual("��������� �.�.", r.Autors[0]);
            Assert.AreEqual("��������� �.�.", r.Autors[1]);
            Assert.AreEqual("�������� A.M.", r.Autors[2]);

            Assert.AreEqual(2006, r.Year);

            Assert.AreEqual("�.", r.Publisher);
            Assert.AreEqual("�������������� ������������ � ������ ����������, ������� �������.", r.Title);

            Assert.AreEqual(null, r.Tom);
            Assert.AreEqual(null, r.Pages.Item1);
            Assert.AreEqual(null, r.Pages.Item2);
            Assert.AreEqual(null, r.PageCount);
        }

        [Test]
        public void Test2()
        {
            string data_in = "(�������� �.�., ������� �.�. ���������� �����������: ������������, ������, �������������. ���., 2002, 202 �.)";
            var r = Ref.Parse(data_in);

            Assert.AreEqual(2, r.Autors.Length, 2);
            Assert.AreEqual("�������� �.�.", r.Autors[0]);
            Assert.AreEqual("������� �.�.", r.Autors[1]);

            Assert.AreEqual("���.", r.Publisher);
            Assert.AreEqual("���������� �����������: ������������, ������, �������������.", r.Title);

            Assert.AreEqual(2002, r.Year);

            Assert.AreEqual(null, r.Tom);
            Assert.AreEqual(null, r.Pages.Item1);
            Assert.AreEqual(null, r.Pages.Item2);
            Assert.AreEqual(202, r.PageCount);
        }

        [Test]
        public void Test3()
        {
            string data_in = "(����� �.�. ������ �������������. �. : �������� ����������, 2006)";
            var r = Ref.Parse(data_in);

            Assert.AreEqual(1, r.Autors.Length);
            Assert.AreEqual("����� �.�.", r.Autors[0]);

            Assert.AreEqual("�. : �������� ����������", r.Publisher);
            Assert.AreEqual("������ �������������.", r.Title);

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
        //    var analyzer = new Analyzer() { Standard = Standard.Get("����-�-7.0.5-2008"), Log = (s) => output.Add(s) };

        //    var refs = Analyzer.ReadRefsFromDocx("../../../IntratextRefTest.docx");
        //    foreach (var r in refs)
        //    {
        //        analyzer.Analyze(r);
        //    }
        //    // ...
        //}
    }
}