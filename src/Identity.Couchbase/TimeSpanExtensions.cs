using System;

namespace Identity.Couchbase
{
    public static class TimeSpanExtensions
    {
        public static uint ToTtl(this TimeSpan duration)
        {
            if (duration <= TimeSpan.FromDays(30))
            {
                return (uint)duration.TotalSeconds;
            }

            var dateExpiry = DateTime.UtcNow + duration;
            var unixTimeStamp = (uint)(dateExpiry.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimeStamp;
        }

        public static uint ToTtl(this uint duration)
        {
            return ToTtl(new TimeSpan(0, 0, 0, 0, (int)duration));
        }


        public static uint NowTtl()
        {
            var datenow = DateTime.UtcNow;
            return (uint)(datenow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}