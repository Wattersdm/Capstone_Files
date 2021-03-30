using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TossAway2.Classes
{
    public enum userTypeId
    {
        Guardian = 1,
        Caretaker = 2,
        Adolescent = 3,
        Child = 4,
        Admin = 5
    }

    public enum relationshipTypeId
    {
        ChildToGuardian = 1,
        ChildToCaretaker = 2,
        ChildToAdolescent = 3,
        ChildToChild = 4
    }
}
