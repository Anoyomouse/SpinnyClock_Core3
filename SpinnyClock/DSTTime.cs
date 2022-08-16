using System;

namespace SpinnyClock
{
    internal class DSTTime
    {
        private readonly DateTime dstStart;
        private readonly DateTime dstEnd;

        protected DSTTime()
        {
            DateTime date = new(DateTime.Now.Year, 4, 1);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(1);

            dstStart = date;

            date = new DateTime(DateTime.Now.Year, 10, 30);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(-1);

            dstEnd = date;
        }

        private static DSTTime _inst;
        public static DSTTime Get
        {
            get
            {
                if (_inst == null)
                    _inst = new DSTTime();
                return _inst;
            }
        }

        public bool Offset(DateTime in_dte)
        {
            if (in_dte.DayOfYear > dstStart.DayOfYear && in_dte.DayOfYear < dstEnd.DayOfYear)
            {
                return true;
            }
            else if (in_dte.DayOfYear == dstStart.DayOfYear)
            {
                if (in_dte.Hour >= 2)
                    return true;
                return false;
            }
            else if (in_dte.DayOfYear == dstEnd.DayOfYear)
            {
                if (in_dte.Hour <= 3)
                    return true;
                return false;
            }
            return false;
        }
    }
}
