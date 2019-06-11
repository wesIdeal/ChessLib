using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public static class UCIOptionsHelper
    {
        public static bool IsOptionValid(this UCIEngineInformation enginInfo, string name, string value)
        {
            var opt = enginInfo.Options.SingleOrDefault(x => x.Name == name);
            if (opt == null) return false;
            var type = opt.GetType();
            if (type == typeof(UCISpinOption))
            {
                return (opt as UCISpinOption).IsSpinOptionValid(value);
            }
            return true;
        }

        private static bool IsSpinOptionValid(this UCISpinOption opt, string value)
        {
            if (!double.TryParse(value, out double dValue))
            {
                return false;
            }
            if (opt.Min.HasValue)
            {
                if (dValue < opt.Min.Value) { return false; }
            }
            if (opt.Max.HasValue)
            {
                if (dValue > opt.Max.Value) { return false; }
            }
            return true;
        }
    }
}
