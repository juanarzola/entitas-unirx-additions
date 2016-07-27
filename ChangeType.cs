using System;

namespace Entitas {

    /** Used in the *AnyChangeObservable methods to observe multiple change types */
    [Flags]
    public enum ChangeType : short{
        Addition = 1 << 0,
        Replacement = 1 << 1,
        Removal = 1 << 2,  
    };
}