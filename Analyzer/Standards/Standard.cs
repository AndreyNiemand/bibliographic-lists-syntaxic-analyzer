using System;
using System.Collections.Generic;

namespace bibliographic_lists_syntaxic_analyzer
{
    public abstract class Standard
    {
        static Standard()
        {
            standards.Add("ГОСТ-Р-7.0.5-2008", () => new ГОСТ_Р_705_2008());
        }

        private List<object> order { get; set; }
        private Dictionary<(object, object), string> separators { get; set; }
        private Dictionary<object, (string, string)> patterns { get; set; }
        private List<string> wrongCondtionsMessages { get; set; }

        protected string DefaultSeparator = " ";

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
            order.Add(r.Autors);
            order.Add(r.Title);
            order.Add(r.Publisher);
            order.Add(r.Tom);
            order.Add(r.Year);
            order.Add(r.Pages);
            order.Add(r.PageCount);

            Check(r);

            return wrongCondtionsMessages;
        }

        public string GetRightRef()
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

            return result;
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

        protected abstract void Check(Ref r);

        protected void Mustbe(bool cond, string message)
        {
            if (!cond)
            {
                wrongCondtionsMessages.Add(message);
            }
        }

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
