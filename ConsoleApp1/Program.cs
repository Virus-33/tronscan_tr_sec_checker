using System.ComponentModel;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

var client = new HttpClient();

string test_hash = "853793d552635f533aa982b92b35b00e63a1c1add062c099da2450a15119bcb2";

Console.WriteLine("Введите хэш проверяемой транзакции ниже, или нажмите Enter для автоматического введения проверочной транзакции:");

string temp_hash = Console.ReadLine();

if (temp_hash.Contains(','))
{
    temp_hash = temp_hash.Substring(0, temp_hash.IndexOf(',')-1);
}

if (temp_hash.Length < 5)
{
    temp_hash = test_hash;
}

HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://apilist.tronscanapi.com/api/security/transaction/data?hashes=" + temp_hash);

string api_key = string.Empty;

using (StreamReader sr = new("key.txt"))
{
    api_key = sr.ReadLine();
}

request.Headers.Add("TRON-PRO-API-KEY", api_key);

HttpResponseMessage response = client.SendAsync(request).Result;

string temp = response.Content.ReadAsStringAsync().Result;

Dictionary<string, Transaction> deserializedContent = JsonSerializer.Deserialize<Dictionary<string, Transaction>>(temp);

Transaction transaction = deserializedContent.Values.First();

if (transaction.riskTransaction)
{
    Console.WriteLine("Это ненадёжная транзакция. Детали:");
    if (transaction.riskToken) Console.WriteLine("Токены этой транзакции не были проверены, или были помечены как мошеннеческие.");
    if (transaction.zeroTransfer) Console.WriteLine("Сумма транзакции нулевая");
    if (transaction.sameTailAttach) Console.WriteLine("Конец адреса пользователя совпадает с концом адреса отправителя");
    if (transaction.riskAddress) Console.WriteLine("Один из адресов не проверен, или был помечен как мошеннический");
} else
{
    Console.WriteLine("Это безопасная транзакция");
}


// Transaction class with desired fields
internal class Transaction
{
    public Transaction(bool riskToken, bool zeroTransfer, bool riskAddress, bool sameTailAttach, bool riskTransaction) 
    {
        this.riskToken = riskToken;
        this.zeroTransfer = zeroTransfer;
        this.riskAddress = riskAddress;
        this.sameTailAttach = sameTailAttach;
        this.riskTransaction = riskTransaction;
    }
    public bool riskToken { get; set; }
    public bool zeroTransfer { get; set; }
    public bool riskAddress { get; set; }
    public bool sameTailAttach { get; set; }
    public bool riskTransaction { get; set; }
}
