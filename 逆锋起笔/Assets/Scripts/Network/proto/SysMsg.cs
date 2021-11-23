using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class MsgPing : MsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }
    }

    public class MsgPong : MsgBase
    {
        public MsgPong()
        {
            protoName = "MsgPong";
        }
    }
}
