using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace bibliographic_lists_syntaxic_analyzer
{
    public abstract class Ref
    {
        public string[] Authors { get; set; }
        public int? Year { get; set; }
        public (uint?, uint?) Pages { get; set; }
        public uint? PageCount { get; set; }
        public uint? Tom { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
    }
}
