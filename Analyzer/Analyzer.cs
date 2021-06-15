namespace bibliographic_lists_syntaxic_analyzer
{
    public class Analyzer
    {
        public Standard Standard { get; set; } = null;

        public delegate void WriteFunction(string s);
        public WriteFunction Log { get; set; } = null;

        public Analyzer() {}

        public void Analyze(Ref r)
        { 
            string maybeRepeat = r.FirstRef != null ? "Repeat " : "";

            Log(string.Format("Analysis: {0}", r.Raw));
            Log("Type: " + maybeRepeat + r.Type.ToString());

            var mistakes = Standard._Check(r);

            if (mistakes.Count != 0)
            {
                Log("Mistake(s): ");
                for (var i = 0; i < mistakes.Count; ++i)
                {
                    Log(string.Format(" {0}) {1}", i + 1, mistakes[i]));
                }
                Log("");
            }

            var rightRef = Standard.GetRightRef(r.Type);
            if (rightRef.Trim() == r.Raw.Trim() && mistakes.Count == 0)
            {
                Log("This reference is correct!");
            }
            else
            {
                Log(string.Format("Character-correct reference: {0}", rightRef));
            }
            Log("--------------------------------------------------------------");
        }

        public void Analyze(string reference)
        {
            var parser = new Parser(Standard);
            if (!parser.TryParse(reference, out Ref r, Ref.PositionType.Intratext))
            {
                return;
            }

            Analyze(r);  
        }
    }
}
