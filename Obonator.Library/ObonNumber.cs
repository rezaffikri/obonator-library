using System;
using System.Text;
using System.Threading;

namespace Obonator.Library
{
    public class ObonNumber
    {
        private static int _seedCount = 0;
        private static ThreadLocal<Random> _tlRng = new ThreadLocal<Random>(() => new Random(GenerateSeed()));

        private static int GenerateSeed()
        {
            // note the usage of Interlocked, remember that in a shared context we can't just "_seedCount++"
            return (int)((DateTime.Now.Ticks << 4) + Interlocked.Increment(ref _seedCount));
        }


        /// <summary>
        /// Get one random number between 0-9999
        /// </summary>
        /// <returns></returns>
        public static long GenerateRandomNumber()
        {
            return _tlRng.Value.Next(9999);
        }

        /// <summary>
        /// Get random number between 0-(lenght)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long GenerateRandomNumber(int length)
        {
            return _tlRng.Value.Next(length);
        }

        public static int HexToInt(string hex)
        {
            int ret = -1;

            try
            {
                ret = Convert.ToInt32(hex, 16);
            }
            catch { }

            return ret;
        }

        public static string IntToHex(int num)
        {
            string ret = "";

            try
            {
                ret = Convert.ToString(num, 16);
            }
            catch { }

            return ret;
        }

        public static DateTime ToDateTime(object strdate)
        {
            DateTime ret = new DateTime();

            try
            {
                ret = Convert.ToDateTime(strdate);
            }

            catch { }

            return ret;
        }

        public static DateTime ToDateTime(string strdate)
        {
            DateTime ret = new DateTime();

            try
            {
                ret = Convert.ToDateTime(strdate);
            }

            catch { }

            return ret;
        }

        public static int ToInt(string strnum)
        {
            int i = 0;

            try
            {
                double d = Convert.ToDouble(strnum);
                i = Convert.ToInt32(d);
            }
            catch
            { }

            return i;
        }

        public static long ToLong(string strnum)
        {
            long i = 0;

            try
            {
                double d = Convert.ToDouble(strnum);
                i = Convert.ToInt64(d);
            }
            catch
            { }

            return i;
        }

        public static double ToDouble(string strnum)
        {
            double ret = 0;

            try
            {
                ret = Convert.ToDouble(strnum);
            }
            catch { }

            return ret;
        }

        public static string FormatAmount(int amt)
        {
            return FormatAmount(amt.ToString());
        }

        public static string FormatAmount(long amt)
        {
            return FormatAmount(amt.ToString());
        }

        public static string FormatAmount(string amt)
        {
            amt = amt.Replace(",", "");
            amt = amt.Replace(".", "");

            long iamt = ToLong(amt);
            string ret = iamt.ToString("##,##");

            if (ret == "")
                ret = "0";

            return ret;
        }
    }
}
