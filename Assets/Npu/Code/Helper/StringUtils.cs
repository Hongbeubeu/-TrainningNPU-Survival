using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

/***
 * @authors: Thanh Le (William)  
 */


namespace Npu.Helper
{
    public static class StringUtils
    {

        /// <summary>
        /// Return 1st, 2nd, 3rd,... of number <paramref name="i"/>
        /// </summary>
        public static string ToOrdinals(int i)
        {
            if (i <= 0) return i.ToString();

            switch (i % 100)
            {
                case 11:
                case 12:
                case 13:
                    return i + "th";
            }

            switch (i % 10)
            {
                case 1:
                    return i + "st";
                case 2:
                    return i + "nd";
                case 3:
                    return i + "rd";
                default:
                    return i + "th";
            }
        }


        /// <summary>
        /// 1 -> I, 10 -> X,.....
        /// </summary>
        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }


        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary>
        public static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target))
                    return 0;
                return target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            var n = source.Length;
            var m = target.Length;
            var d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (var i = 0; i <= n; d[i, 0] = i++) ;
            for (var j = 1; j <= m; d[0, j] = j++) ;

            for (var i = 1; i <= n; i++)
            {
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    var min1 = d[i - 1, j] + 1;
                    var min2 = d[i, j - 1] + 1;
                    var min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }



        /// <summary>
        /// Return hash256 of string
        /// </summary>
        public static string FastHash(string str)
        {
            return _HashSHA256(str);
        }

        static readonly SHA256 _SHA = SHA256.Create();
        static string _HashSHA256(string str)
        {
            var hashBytes = _SHA.ComputeHash(Encoding.UTF8.GetBytes(str));
            var builder = new StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }



        const int _ShortFormatDefaultMaxDecimals = 2;
        const string _ShortFormatDefaultFormat = "{0}{1}";

        public static string ShortFormat(double d, int maxDecimal = _ShortFormatDefaultMaxDecimals, string format = _ShortFormatDefaultFormat)
        {
            if (double.IsNaN(d) || double.IsInfinity(d)) return d.ToString();

            var thouPow = 0;
            var sign = d >= 0 ? 1 : -1;
            var ddiv = Math.Abs(d);
            while (ddiv >= 1000)
            {
                ddiv /= 1000;
                thouPow++;
            }
            var suffix = new StringBuilder();
            if (thouPow > 0)
            {
                if (thouPow - 1 < _sfChars1.Length) suffix.Append(_sfChars1[thouPow - 1]);
                else
                {
                    var t = thouPow - 1 - _sfChars1.Length;
                    var l = _sfChars2.Length;
                    while (t >= 0)
                    {
                        var n = t % l;
                        suffix.Insert(0, _sfChars2[n]);
                        t = t / l - 1;
                    }
                }
            }
            return string.Format(format, sign * Math.Round(ddiv, maxDecimal), suffix);
        }

        public static string ShortFormat(BigInteger d, int maxDecimal = 0, string format = "{0}{1}")
        {
            var thouPow = 0;
            var sign = d.Sign;
            var abs = BigInteger.Abs(d);
            var ddiv = abs + 0;
            while (ddiv >= 1000)
            {
                ddiv /= 1000;
                thouPow++;
            }
            var prefix = new StringBuilder();
            prefix.Append(sign * ddiv);
            if (maxDecimal > 0)
            {
                var tens = BigInteger.Pow(10, maxDecimal);
                var thous = BigInteger.Pow(1000, thouPow);
                var d2 = abs * tens;
                var d2base = d2 / thous;
                var d2floor = (d2 / thous / tens) * tens;
                var d2rem = d2base - d2floor;
                if (d2rem > 0)
                {
                    prefix.Append('.');
                    prefix.Append(d2rem.ToString().TrimEnd('0'));
                }
            }
            var suffix = new StringBuilder();
            if (thouPow > 0)
            {
                if (thouPow - 1 < _sfChars1.Length) suffix.Append(_sfChars1[thouPow - 1]);
                else
                {
                    var t = thouPow - 1 - _sfChars1.Length;
                    var l = _sfChars2.Length;
                    while (t >= 0)
                    {
                        var n = t % l;
                        suffix.Insert(0, _sfChars2[n]);
                        t = t / l - 1;
                    }
                }
            }
            return string.Format(format, prefix, suffix);
        }

        static readonly char[] _sfChars1 = "KMBT".ToCharArray();
        static readonly char[] _sfChars2 = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    }
}
