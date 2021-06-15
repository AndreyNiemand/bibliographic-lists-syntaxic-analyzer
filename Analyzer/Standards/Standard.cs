
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{ 
    public abstract class Standard
    {
        static Standard()
        {
            standards.Add("ГОСТ-Р-7.0.5-2008", () => new ГОСТ_Р_705_2008());
        }

        public abstract Regex PublisherRegex { get; } 
        public abstract Regex TitleRegex { get; } 
        public abstract Regex TomRegex { get; }
        public abstract Regex AuthorsRegex { get; }
        public abstract Regex YearRegex { get; }
        public abstract Regex PagesRegex { get; } 
        public abstract Regex PagesCountRegex { get; }

        public abstract string ComplexSeparator { get; }
        public virtual string Ibid { get; } = "Ibid";

        public virtual bool EnoughInfoToBeParsed(Ref r)
        {
            return (r.Authors != null || r.Year != null) && r.Title != null;
        }

        public abstract bool IsRepeatRef(Ref first, Ref repeat);
        public abstract bool IsPartOfOfftextRef(Ref part, Ref r);

        private List<object> order { get; set; }
        private Dictionary<(object, object), string> separators { get; set; }
        private Dictionary<object, (string, string)> patterns { get; set; }
        private List<string> wrongCondtionsMessages { get; set; }

        protected string DefaultSeparator { get; set; } = " ";
        public virtual string OfftextRefNameOfHeading => "Offtext References";

        private static Dictionary<string, Func<Standard>> standards = new Dictionary<string, Func<Standard>>();

        public static Standard Get(string standardName)
        { 
            if (standards.TryGetValue(standardName, out Func<Standard> constructStandard))
            {
                return constructStandard();
            }

            throw new UnknownStandardException(standardName);
        }

        public List<string> _Check(Ref r)
        {
            separators = new Dictionary<(object, object), string>();
            wrongCondtionsMessages = new List<string>();
            patterns = new Dictionary<object, (string, string)>();

            order = new List<object>();
            order.Add(r.Authors);
            order.Add(r.Title);
            order.Add(r.Publisher);
            order.Add(r.Tom);
            order.Add(r.Year);
            order.Add(r.Pages);
            order.Add(r.PageCount);

            Check(r);

            return wrongCondtionsMessages;
        }

        protected virtual (string, string) IntratextRefCharsAround => ("", "");
        protected virtual (string, string) SubscriptRefCharsAround => ("", "");
        protected virtual (string, string) OfftextRefCharsAround => ("", "");
        protected virtual (string, string) OfftextPartRefCharsAround => ("", "");

        public string GetRightRef(Ref.PositionType type)
        {
            string result = "";
            object previous = null;

            foreach (var val in order)
            {
                if (val != null)
                {
                    if (val.GetType() == typeof(string[]))
                    {
                        var objs = (string[])val;

                        result += SeparatorBetween(previous, objs) + ToStringWithPattern(objs[0]);
                        for (var i = 1; i < objs.Length; ++i)
                        {
                            result += SeparatorBetween(objs[i - 1], objs[i]) + ToStringWithPattern(objs[i]);
                        }
                    }
                    else if (val.GetType() == typeof((uint?, uint?)))
                    {
                        var (a, b) = ((uint?, uint?))val;
                        if (a != null)
                        {
                            result += SeparatorBetween(previous, val) + ToStringWithPattern(a);
                            if (b != null)
                            {
                                result += SeparatorBetween(a, b) + ToStringWithPattern(b);
                            }
                        }

                        if (a == null || b == null) continue;
                    }
                    else
                    {
                        result += SeparatorBetween(previous, val) + ToStringWithPattern(val);
                    }

                    previous = val;
                }
            }

            switch (type)
            {
                case Ref.PositionType.Intratext:   return IntratextRefCharsAround.  Item1 + result + IntratextRefCharsAround.  Item2;
                case Ref.PositionType.Subscript:   return SubscriptRefCharsAround.  Item1 + result + SubscriptRefCharsAround.  Item2;
                case Ref.PositionType.Offtext:     return OfftextRefCharsAround.    Item1 + result + OfftextRefCharsAround.    Item2;
                case Ref.PositionType.OfftextPart: return OfftextPartRefCharsAround.Item1 + result + OfftextPartRefCharsAround.Item2;
            }

            throw new NotImplementedException();
        }

        private string ToStringWithPattern(object obj)
        {
            (string, string) p;
            if (patterns.TryGetValue(obj, out p))
            {
                var (before, after) = p;
                return before + obj.ToString() + after;
            }

            return obj.ToString();
        }

        private string SeparatorBetween(object obj1, object obj2)
        {
            if (obj1 == null)
                return "";

            if (separators.TryGetValue((obj1, obj2), out string result))
                return result;

            return DefaultSeparator;
        }

        protected virtual void Check(Ref r) { }

        protected void Log(string message) => wrongCondtionsMessages.Add(message);

        protected void Separate(object obj1, string separator, object obj2)
        {
            this.separators[(obj1, obj2)] = separator;
        }

        protected void Separate(object[] objs, string separator)
        {
            if (objs == null) return;

            for (var i = 0; i < objs.Length - 1; ++i)
            {
                this.separators[(objs[i], objs[i+1])] = separator;
            }
        }
        protected void Separate((object, object) objs, string separator)
        {
            this.separators[objs] = separator;
        }

        protected void Pattern(string before, object obj, string after = "")
        {
            if (obj != null)
                patterns.Add(obj, (before, after));
        }
        protected void Pattern(string before, (object, object) objs, string after = "")
        {
            var (o1, o2) = objs;
            if (o1 != null)
                patterns.Add(o1, (before, ""));

            if (o2 != null)
                patterns.Add(o2, ("", after));
        }

        protected void Pattern(object obj, string after)
        {
            Pattern("", obj, after);
        }

        protected void Order(object obj, int n)
        {
            int key = 0;
            object val = null;

            foreach (var v in order)
            {
                if (v != obj)
                {
                    key++;
                }
                else
                {
                    val = v;
                    break;
                }
            }

            order[key] = order[n];
            order[n] = obj;
        }
    }
}
