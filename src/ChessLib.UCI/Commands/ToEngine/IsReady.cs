using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.UCI.Commands.ToEngine
{
    public class IsReady : CommandInfo
    {
        public IsReady() : base(AppToUCICommand.IsReady)
        {

        }
    }
}
