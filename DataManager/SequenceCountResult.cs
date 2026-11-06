using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal class SequenceCountResult
    {
        internal DateTime Date;

        internal int? Line1_CarType_1;
        internal int? Line1_CarType_2;
        internal int? Line1_CarType_3;
        internal int? Line2_CarType_1;
        internal int? Line2_CarType_2;
        internal int? Line2_CarType_3;

        public SequenceCountResult() { }

        //Methods
        //-----------------------------------------------------
        internal int? getLine1Total()
        {
            if (Line1_CarType_1 == null) { Line1_CarType_1 = 0; }
            if (Line1_CarType_2 == null) { Line1_CarType_2 = 0; }
            if (Line1_CarType_3 == null) { Line1_CarType_3 = 0; }

            return Line1_CarType_1 + Line1_CarType_2 + Line1_CarType_3;
        }

        internal int? getLine2Total()
        {
            if (Line2_CarType_1 == null) { Line2_CarType_1 = 0; }
            if (Line2_CarType_2 == null) { Line2_CarType_2 = 0; }
            if (Line2_CarType_3 == null) { Line2_CarType_3 = 0; }

            return Line2_CarType_1 + Line2_CarType_2 + Line2_CarType_3;
        }
    }
}
