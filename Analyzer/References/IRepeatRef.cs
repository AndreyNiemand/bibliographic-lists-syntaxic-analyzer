using System;
using System.Collections.Generic;
using System.Text;

namespace bibliographic_lists_syntaxic_analyzer
{
    public interface IRepeatRef
    {
        List<Ref> ChildReferences { get; set; }
    }
}
