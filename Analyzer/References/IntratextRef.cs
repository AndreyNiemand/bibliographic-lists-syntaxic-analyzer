using System.Collections.Generic;

namespace bibliographic_lists_syntaxic_analyzer
{
    public class IntratextRef : Ref
    {
        public IntratextRef()
        { 
            
        }
    }

    public class RepeatIntratextRef : IntratextRef, IRepeatRef
    {
        public RepeatIntratextRef(Ref r)
        {
            Init(r);
        }

        public List<Ref> ChildReferences { get; set; } = new List<Ref>();
    }
}
