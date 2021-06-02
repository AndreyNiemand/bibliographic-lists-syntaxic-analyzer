using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{ 
    class UnknownStandardException : Exception
    {
        public UnknownStandardException(string std): base("Unknown Standard: " + std + ". ")
        { 
            
        }
    }
}
