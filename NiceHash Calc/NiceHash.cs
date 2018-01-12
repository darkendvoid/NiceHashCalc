using System;
using System.Text;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Flurl;
using Flurl.Http;
using System.Configuration;

namespace NiceHashCalc
{   
    //MAIN PROGRAM
    class NiceHash
    {
        static void Main(string[] args)
        {   //UPDATE TO YOUR OWN VALUES
			string CoinbaseAPIKey = ConfigurationManager.AppSettings["CoinbaseAPIKey"];
            string CoinbaseAPISecret = ConfigurationManager.AppSettings["CoinbaseAPISecret"];
			string CoinbaseAccount = ConfigurationManager.AppSettings["CoinbaseAccount"];
			string NiceHashAddress = ConfigurationManager.AppSettings["NiceHashAddress"];
			string NiceHashAPIId = ConfigurationManager.AppSettings["NiceHashAPIId"];
			string NiceHashAPIKey = ConfigurationManager.AppSettings["NiceHashAPIKey"];
            double TargetProfits = Convert.ToDouble(ConfigurationManager.AppSettings["TargetProfits"]);
            DateTime StartDate = Convert.ToDateTime(ConfigurationManager.AppSettings["StartDate"]);
            DateTime TargetProfitDate = Convert.ToDateTime(ConfigurationManager.AppSettings["TargetProfitDate"]);
            string CBCurrency = Convert.ToString(ConfigurationManager.AppSettings["FiatCurrency"]);
            //BEGIN EXECUTION LOOP
            while (true)
            {
                Console.Clear();
                ConsoleOutput(CoinbaseAPIKey, CoinbaseAPISecret, CoinbaseAccount, NiceHashAddress, TargetProfits, StartDate, TargetProfitDate, NiceHashAPIId, NiceHashAPIKey, CBCurrency);
                Thread.Sleep(120000);
            }
        }

        static void ConsoleOutput(string CBAPIKEY, string CBSECRETKEY, string CBACCOUNT, string NiceHashAddress, double TargetProfits, DateTime StartDate, DateTime TargetProfitDate, string NiceHashAPIId, string NiceHashAPIKey, string CBCurrency)
        {
            //DO NOT TOUCH BELOW
            double CBBalance = GetCBBalance(CBAPIKEY, CBSECRETKEY, CBACCOUNT);
            double NHBalance = GetNicehashBalance(NiceHashAddress);
            double CURBTC = SpotSellPrice(CBAPIKEY, CBSECRETKEY, CBCurrency);
            double NHWalletBalance = GetNicehashWallet(NiceHashAPIId, NiceHashAPIKey);
            double TotalBTC = (NHBalance + NHWalletBalance + CBBalance);
            DateTime Today = DateTime.Now;
            TimeSpan DaysUntilPayment = TargetProfitDate.Subtract(Today);
            TimeSpan DaysMining = Today.Subtract(StartDate);
            double TotalMinutesMining = ((((DaysMining.Days * 24) + DaysMining.Hours) * 60) + DaysMining.Minutes);
            double ProfitPerMin = Math.Round(((((NHBalance + CBBalance + NHWalletBalance) / TotalMinutesMining))), 8);
            double ROI = Math.Round((TargetProfits / (((ProfitPerMin * 60) * 24) * CURBTC)), 2);
            double BufferDays = Math.Floor(DaysUntilPayment.Days - ROI);
            DateTime EstimatedPaymentDate = Today.AddDays(ROI);
            string strCURBTC = CURBTC.ToString("#,##0.##") + " " + CBCurrency;
            string strCBBalance = ((CBBalance) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strNHBalance = ((NHBalance + NHWalletBalance) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strTotalBalance = ((CBBalance + NHBalance + NHWalletBalance) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strBTCPerDay = ((ProfitPerMin * 60) * 24).ToString("N8");
            string strPPH = ((ProfitPerMin * 60) * CURBTC).ToString("0.00000") + " " + CBCurrency;
            string strPPD = (((ProfitPerMin * 60) * 24) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strPPW = ((((ProfitPerMin * 60) * 24) * 7) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strPPM = ((((ProfitPerMin * 60) * 24) * 30) * CURBTC).ToString("#,##0.##") + " " + CBCurrency;
            string strProfitTarget = TargetProfits.ToString("#,##0.##") + " " + CBCurrency;
            string strDaysUntilTarget = DaysUntilPayment.Days.ToString("N2");
            string strNHBTC = NHBalance.ToString("N8");
            string strNHWallet = NHWalletBalance.ToString("N8");
            string strCBBTC = CBBalance.ToString("N8");
            string strTotalBTC = TotalBTC.ToString("N8");
            Console.WriteLine("╔══════════════════════════════════════════════════════════╦═════════════════════════════════════════════════════════╗");
            Console.WriteLine("║ NiceHash Balance:" + strNHBTC.PadLeft(35, ' ') + " BTC" + " ║ " + "BTC Price:" + strCURBTC.PadLeft(45, ' ') +" ║");
            Console.WriteLine("║ NiceHash Wallet Balance:" + strNHWallet.PadLeft(28,' ') + " BTC" + " ║ " + "Coinbase Balance:" + strCBBalance.PadLeft(38, ' ')+" ║");
            Console.WriteLine("║ Coinbase Balance:" + strCBBTC.PadLeft(35, ' ') + " BTC" + " ║ " + "NiceHash Balance:" + strNHBalance.PadLeft(38, ' ')+" ║");
            Console.WriteLine("║                                                          ║                                                         ║");
            Console.WriteLine("║ Total Balance:" + strTotalBTC.PadLeft(38, ' ') + " BTC" + " ║ " + "Total Balance:" + strTotalBalance.PadLeft(41, ' ')+ " ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╬═════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ Proit Target:" + strProfitTarget.PadLeft(43, ' ') + " ║ " + "Bitcoin Per Day:" + strBTCPerDay.PadLeft(35, ' ')+ " BTC ║");
            Console.WriteLine("║ Mining Start Date:                            " + StartDate.ToString("MM/dd/yyyy") + " ║ " + "Profit Per Hour:" + strPPH.PadLeft(39, ' ') +  " ║");
            Console.WriteLine("║ Profits Target Date:                          " + TargetProfitDate.ToString("MM/dd/yyyy") + " ║ " + "Profit Per Day:" + strPPD.PadLeft(40, ' ') +" ║");
            Console.WriteLine("║ Days Until Target:" + strDaysUntilTarget.PadLeft(38, ' ')+" ║ " + "Profit Per Week:" + strPPW.PadLeft(39, ' ') + " ║");
            Console.WriteLine("║ Estimated Target Date:                        " + EstimatedPaymentDate.ToString("MM/dd/yyyy") + " ║ " + "Profit Per Month:" +  strPPM.PadLeft(38, ' ') +" ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╩═════════════════════════════════════════════════════════╝");
        }

        //COINBASE FUNCTIONS
        static double SpotSellPrice(string apiKey, string apiSecret, string apiCurrency)
        {
            var host = "https://api.coinbase.com/";
            var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var currency = apiCurrency;
            var message = string.Format("{0}GET/v2/prices/sell?currency={1}", unixTimestamp.ToString(), currency);

            byte[] secretKey = Encoding.UTF8.GetBytes(apiSecret);
            HMACSHA256 hmac = new HMACSHA256(secretKey);
            hmac.Initialize();
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] rawHmac = hmac.ComputeHash(bytes);
            var signature = rawHmac.ByteArrayToHexString();

            var price = host
                 .AppendPathSegment("v2/prices/sell")
                 .SetQueryParam("currency", currency)
                 .WithHeader("CB-ACCESS-SIGN", signature)
                 .WithHeader("CB-ACCESS-TIMESTAMP", unixTimestamp)
                 .WithHeader("CB-ACCESS-KEY", apiKey)
                 .GetJsonAsync<dynamic>()
                 .Result;
            CBData response = JsonConvert.DeserializeObject<CBData>(price.ToString(Formatting.None));
            var USDPrice = Convert.ToDouble(response.data.amount);
            return USDPrice;
        }
        static double GetCBBalance(string apiKey, string apiSecret, string Account)
        {
            var host = "https://api.coinbase.com/";
            var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var currency = "BTC";
            var message = string.Format("{0}GET/v2/accounts/" + Account, unixTimestamp.ToString(), currency);

            byte[] secretKey = Encoding.UTF8.GetBytes(apiSecret);
            HMACSHA256 hmac = new HMACSHA256(secretKey);
            hmac.Initialize();
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] rawHmac = hmac.ComputeHash(bytes);
            var signature = rawHmac.ByteArrayToHexString();

            var account = host
                .AppendPathSegment("v2/accounts/" + Account)
                .WithHeader("CB-ACCESS-SIGN", signature)
                .WithHeader("CB-ACCESS-TIMESTAMP", unixTimestamp)
                .WithHeader("CB-ACCESS-KEY", apiKey)
                .GetJsonAsync<dynamic>()
                .Result;
            CBPriceData response = JsonConvert.DeserializeObject<CBPriceData>(account.ToString(Formatting.None));
            var AccountBalance = Convert.ToDouble(response.data.balance.amount);
            return AccountBalance;
        }
        //NICEHASH FUNCTIONS
        static double GetNicehashBalance(string Address)
        {
            double NHB = 0;
            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    var json = webClient.DownloadString("https://api.nicehash.com/api?method=stats.provider&addr=" + Address);
                    NHMain models = JsonConvert.DeserializeObject<NHMain>(json);
                    for (var x = 0; x < models.result.stats.Count; x++)
                    {
                        NHB = NHB + Convert.ToDouble(models.result.stats[x].balance);
                    }
                }
                catch (WebException e)
                {
                    NHB = 0;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("╠════════════════════════════════════════NICE HASH API IS DOWN═════════════════════════════════════╣");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════════════════════╝");
                }
            }
            return NHB;
        }
        static double GetNicehashWallet(string Id, string Key)
        {
            double NHW = 0;
            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    var json = webClient.DownloadString("https://api.nicehash.com/api?method=balance&id=" + Id + "&key=" + Key);
                    NHWallet models = JsonConvert.DeserializeObject<NHWallet>(json);
                    NHW = NHW + Convert.ToDouble(models.result.balance_confirmed)+ Convert.ToDouble(models.result.balance_pending);                    
                }
                catch (WebException e)
                {
                    NHW = 0;
                }
            }
            return NHW;
        }
    }
}
