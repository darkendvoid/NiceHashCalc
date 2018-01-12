# NiceHashCalc

**DO NOT SHARE APP.CONFIG or NICEHASH CALC.EXE.CONFIG UNLESS YOUR KEYS ARE READ ONLY**
Description:
This application is a console applictaion that gets your current BTC balance on Coinbase and your unpaid balance on NiceHash and performs calulations to project earnings. This calculates your income based on your actualized returns into coinbase and not current market estimates. BTC price is Coinbase sell price for 1 BTC and includes the Coinbase sell fee so it may appear below current market trading price.  

![image](https://user-images.githubusercontent.com/20748167/34861220-efb3b052-f728-11e7-994d-8f14cebb4003.png)
![image](https://user-images.githubusercontent.com/20748167/34861241-2010c776-f729-11e7-89b9-22bd07e1485d.png)


1. Download Visual Studio Community https://www.visualstudio.com/vs/community/
2. Create Coinbase API Keys with wallet:accounts:read permissions on your main BTC wallet
3. Create NiceHash API Keys by going to NiceHash -> Settings -> API and create read only key
4. Open the project in visual studio and modify App.Config -

CoinbaseAPIKey - This should be your API Key from Coinbase

CoinbaseAPISecret - This is your secret key from Coinbase

CoinbaseAccount - This is your Coinbase Account Number, you can get this by going to your accounts tab and clicking on the main BTC wallet. Copy what follows https://www.coinbase.com/accounts/ in the URL bar of your browser. 

NiceHashAddress - This is the mining address your are using at NiceHash

NiceHashAPIId - This is the API ID from your Settings > API page on Nicehash

NiceHashAPIKey - This is the Read Only key from your Settings > API page on Nicehash

TargetProfits - This is how much money you want to make to enable date calculations

StartDate - The day you started mining and were receiving payouts to Coinbase

TargetProfitDate - The date you want to reach your Target Profit By

FiatCurrency - Allows you to use a your local currency for calulations. Please note that your country must be supported by Coinbase for the currency pair to work - https://www.coinbase.com/global . Unlisted countries may work but are not guaranteed.   You must use the ISO 4217 Alphabetic code. https://www.iso.org/iso-4217-currency-codes.html https://en.wikipedia.org/wiki/ISO_4217#Active_codes

5. Build the application and copy all DLL, EXE, and .config from the Bin/Release directory that was created to another folder and run the EXE file. 





Alternatively -

1. Download The Release
2. Modify The Nicehash Calc.exe.config using your favorite text editor to match the App.Config above
3. Enjoy!



Coinbase Implementation was taken from - https://github.com/iYalovoy/demibyte.coinbase

App.Config implemntation taken from - https://github.com/jgulick48
