using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace NiceHashCalc
{
    //NICEHASH JSON CLASSES
    public class NHStat
    {
        public string balance { get; set; }
    }
    public class NHResult
    {
        public List<NHStat> stats { get; set; }
    }
    public class NHMain
    {
        public NHResult result { get; set; }
    }
    public class NHBalance
    {
        public string balance_confirmed { get; set; }
        public string balance_pending { get; set; }
    }
    public class NHWallet
    {
        public NHBalance result { get; set; }
    }
    //COINBASE JSON CLASSES
    public class CBPrice
    {
        public string amount { get; set; }
    }
    public class CBData
    {
        public CBPrice data { get; set; }
    }
    public class CBPriceData
    {
        public CBBalance data { get; set; }
    }
    public class CBBalance
    {
        public CBPrice balance { get; set; }
    }
    //ENCRYPTION CLASSES
    public static class StringHelper
    {
        public static byte[] HexStringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ByteArrayToHexString(this byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}