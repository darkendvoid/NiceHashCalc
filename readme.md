# NiceHashCalc
Description:
This application is console applictaion that gets your current BTC balance on Coinbase and your unpaid balance on NiceHash and performs calulations to project earnings. This calculates your income based on your actualized returns into coinbase and not current market estimates. BTC price is Coinbase sell price for 1 BTC and includes the Coinbase sell fee so it may appear below current market trading price.  

![image](https://user-images.githubusercontent.com/20748167/34656980-230ac110-f3e7-11e7-9e85-62d643925b4b.png)

1. Download Visual Studio Community https://www.visualstudio.com/vs/community/
2. Create Coinbase API Keys with wallet:accounts:read permissions on your main BTC wallet
3. Create NiceHash API Keys by going to NiceHash -> Settings -> API and create read only key
4. Open the project in visual studio and modify in Nicehash.cs -

CoinbaseAPIKey - This should be your API Key from Coinbase

CoinbaseAPISecret - This is your secret key from Coinbase

CoinbaseAccount - This is your Coinbase Account Number, you can get this by going to your accounts tab and clicking on the main BTC wallet. Copy what follows https://www.coinbase.com/accounts/ in the URL bar of your browser. 

NiceHashAddress - This is the mining address your are using at NiceHash

NiceHashAPIId - This is the API ID from your Settings > API page on Nicehash

NiceHashAPIKey - This is the Read Only key from your Settings > API page on Nicehash

TargetProfits - This is how much money you want to make to enable date calculations

StartDate - The day you started mining and were receiving payouts to Coinbase

TargetProfitDate - The date you want to reach your Target Profit By

5. Build the application and copy all DLL and exe from the Bin/Release directory that was created to another folder and run the EXE file. 

Coinbase Implementation was taken from - https://github.com/iYalovoy/demibyte.coinbase
