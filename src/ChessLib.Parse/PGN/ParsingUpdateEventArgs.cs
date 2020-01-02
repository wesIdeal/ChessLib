using System;
using System.Globalization;

namespace ChessLib.Parse.PGN
{
    public enum TimePeriod
    {
        Ms,
        S,
        M
    }

    public class ParsingUpdateEventArgs : EventArgs
    {
        private string _labelOverride;

        public ParsingUpdateEventArgs(TimeSpan elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

        public ParsingUpdateEventArgs(string labelOverride)
        {
            ElapsedTime = null;
            IsIndeterminate = false;
            _labelOverride = labelOverride;

        }

        public int NumberComplete { get; set; }
        public int Maximum { get; set; }
        public TimeSpan? ElapsedTime { get; set; }
        public string Label => string.IsNullOrWhiteSpace(_labelOverride) ? $"{NumberComplete} games complete of {Maximum}"
            : _labelOverride;
        public bool IsIndeterminate { get; set; }

        public string LabelByTimeUnit(TimePeriod period)
        {
            if (!string.IsNullOrWhiteSpace(_labelOverride))
            {
                return _labelOverride;
            }
            var elapsedTimeStr = GetElapsedTimeString(period);
            if (elapsedTimeStr != "")
            {
                elapsedTimeStr = " in " + elapsedTimeStr;
            }
            return Label + elapsedTimeStr;
        }

        protected string GetElapsedTimeString(TimePeriod period)
        {
            if (!ElapsedTime.HasValue)
            {
                return "";
            }

            var et = ElapsedTime.Value;
            switch (period)
            {
                case TimePeriod.Ms:
                    return Math.Round(et.TotalMilliseconds).ToString(CultureInfo.CurrentCulture) + " ms";
                case TimePeriod.S:
                    return et.TotalSeconds.ToString(CultureInfo.CurrentCulture) + " sec";
                case TimePeriod.M:
                    return et.TotalMinutes.ToString(CultureInfo.CurrentCulture) + " min";
                default:
                    throw new ArgumentOutOfRangeException(nameof(period), period, null);
            }
        }
    }
}