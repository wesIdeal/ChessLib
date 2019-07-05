using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.UCI.Commands.FromEngine
{
    public class ErrorResponseArgs : EngineResponseArgs
    {
        public ErrorResponseArgs(string response) : base(response)
        {
        }
    }
}
