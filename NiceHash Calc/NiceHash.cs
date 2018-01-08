using System;
using System.Text;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Flurl;
using Flurl.Http;

namespace NiceHashCalc
{   
    //MAIN PROGRAM
    class NiceHash
    {
        static void Main(string[] args)
        {   //UPDATE TO YOUR OWN VALUES
            string CoinbaseAPIKey = "ThisIsYourCoinbaseAPIKey";
            string CoinbaseAPISecret = "ThisIsYourCoinbaseAPISecret";
            string CoinbaseAccount = "ThisIsYourCoinbaseAccount";
            string NiceHashAddress = "ThisIsYourNiceHashMiningAddress";
            string NiceHashAPIId = "ThisIsyourNiceHashAPIId";
            string NiceHashAPIKey = "ThisIsYourNiceHashAPIReadOnlyKey";
            double TargetProfits = 1000000.00;
            DateTime StartDate = new DateTime(2018, 01, 01);
            DateTime TargetProfitDate = new DateTime(2030, 12, 31);
            //BEGIN EXECUTION LOOP
            while (true)
            {
                Console.Clear();
                ConsoleOutput(CoinbaseAPIKey, CoinbaseAPISecret, CoinbaseAccount, NiceHashAddress, TargetProfits, StartDate, TargetProfitDate, NiceHashAPIId, NiceHashAPIKey);
                Thread.Sleep(120000);
            }
        }

        static void ConsoleOutput(string CBAPIKEY, string CBSECRETKEY, string CBACCOUNT, string NiceHashAddress, double TargetProfits, DateTime StartDate, DateTime TargetProfitDate, string NiceHashAPIId, string NiceHashAPIKey)
        {
            //DO NOT TOUCH BELOW
            double CBBalance = GetCBBalance(CBAPIKEY, CBSECRETKEY, CBACCOUNT);
            double NHBalance = GetNicehashBalance(NiceHashAddress);
            double BTCUsd = SpotSellPrice(CBAPIKEY, CBSECRETKEY);
            double NHWalletBalance = GetNicehashWallet(NiceHashAPIId, NiceHashAPIKey);
            double TotalBTC = (NHBalance + CBBalance);
            DateTime Today = DateTime.Now;
            TimeSpan DaysUntilPayment = TargetProfitDate.Subtract(Today);
            TimeSpan DaysMining = Today.Subtract(StartDate);
            double TotalMinutesMining = ((((DaysMining.Days * 24) + DaysMining.Hours) * 60) + DaysMining.Minutes);
            double ProfitPerMin = Math.Round(((((NHBalance + CBBalance + NHWalletBalance) / TotalMinutesMining))), 8);
            double ROI = Math.Round((TargetProfits / (((ProfitPerMin * 60) * 24) * BTCUsd)), 2);
            double BufferDays = Math.Floor(DaysUntilPayment.Days - ROI);
            DateTime EstimatedPaymentDate = Today.AddDays(ROI);
            Console.WriteLine("╔═════════════════════════════════════════════════╦════════════════════════════════════════════════╗");
            Console.WriteLine("║ NiceHash Balance:                " + NHBalance.ToString("N8") + " BTC" + " ║   " + "BTC Price In USD:                 $ " + BTCUsd.ToString("0000.00") + " ║");
            Console.WriteLine("║ NiceHash Wallet Balance:         " + NHWalletBalance.ToString("N8") + " BTC" + " ║   " + "Coinbase Balance In USD:           $ " + ((CBBalance) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("║ Coinbase Balance:                " + CBBalance.ToString("N8") + " BTC" + " ║   " + "NiceHash Balance In USD:           $ " + ((NHBalance + NHWalletBalance) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("║                                                 ║   " + "                                             ║");
            Console.WriteLine("║ Total Balance:                   " + TotalBTC.ToString("N8") + " BTC" + " ║   " + "Total Balance In USD:              $ " + ((CBBalance + NHBalance + NHWalletBalance) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("╠═════════════════════════════════════════════════╬════════════════════════════════════════════════╣");
            Console.WriteLine("║ Proit Target:                      $ " + TargetProfits.ToString("0000000.00") + " ║   " + "Bitcoin Per Day:              " + ((ProfitPerMin * 60) * 24).ToString("N8") + " BTC ║");
            Console.WriteLine("║ Mining Start Date:                   " + StartDate.ToString("MM/dd/yyyy") + " ║   " + "Profit Per Hour:                   $ " + ((ProfitPerMin * 60) * BTCUsd).ToString("0.00000") + " ║");
            Console.WriteLine("║ Profits Target Date:                 " + TargetProfitDate.ToString("MM/dd/yyyy") + " ║   " + "Profit Per Day:                    $ " + (((ProfitPerMin * 60) * 24) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("║ Days Until Target:                     " + DaysUntilPayment.Days.ToString("00000.00") + " ║   " + "Profit Per Week:                   $ " + ((((ProfitPerMin * 60) * 24) * 7) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("║ Estimated Target Date:               " + EstimatedPaymentDate.ToString("MM/dd/yyyy") + " ║   " + "Profit Per Month:                  $ " + ((((ProfitPerMin * 60) * 24) * 30) * BTCUsd).ToString("0000.00") + " ║");
            Console.WriteLine("╚═════════════════════════════════════════════════╩════════════════════════════════════════════════╝");
        }

        //COINBASE FUNCTIONS
        static double SpotSellPrice(string apiKey, string apiSecret)
        {
            var host = "https://api.coinbase.com/";
            var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var currency = "USD";
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
