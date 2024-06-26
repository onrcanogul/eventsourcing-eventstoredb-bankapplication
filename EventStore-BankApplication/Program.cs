
using EventStore.Client;
using EventStore_BankApplication;
using System.Text.Json;

AccountCreatedEvent accountCreatedEvent = new()
{
    AccountId = "12345",
    CostumerId = "98765",
    StartBalance = 0,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent1 = new()
{
    AccountId = "12345",
    Amount = 1000,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent2 = new()
{
    AccountId = "12345",
    Amount = 500,
    Date = DateTime.UtcNow.Date
};
MoneyWithdrawnEvent moneyWithdrawnEvent = new()
{
    AccountId = "12345",
    Amount = 200,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent3 = new()
{
    AccountId = "12345",
    Amount = 50,
    Date = DateTime.UtcNow.Date
};
MoneyTransferredEvent moneyTransferredEvent1 = new()
{
    AccountId = "12345",
    Amount = 250,
    Date = DateTime.UtcNow.Date
};
MoneyTransferredEvent moneyTransferredEvent2 = new()
{
    AccountId = "12345",
    Amount = 150,
    Date = DateTime.UtcNow.Date
};
MoneyDepositedEvent moneyDepositedEvent4 = new()
{
    AccountId = "12345",
    Amount = 2000,
    Date = DateTime.UtcNow.Date
};

EventStoreService eventStoreService = new();

await eventStoreService.AppendToStreamAsync(
    streamName: $"customer-{accountCreatedEvent.CostumerId}-stream",
    eventData: new[] {
        eventStoreService.GenerateEventData(accountCreatedEvent),
        eventStoreService.GenerateEventData(moneyDepositedEvent1),
        eventStoreService.GenerateEventData(moneyDepositedEvent2),
        eventStoreService.GenerateEventData(moneyWithdrawnEvent),
        eventStoreService.GenerateEventData(moneyDepositedEvent3),
        eventStoreService.GenerateEventData(moneyTransferredEvent1),
        eventStoreService.GenerateEventData(moneyTransferredEvent2),
        eventStoreService.GenerateEventData(moneyDepositedEvent4)
    }
    );

BalanceInfo balanceInfo = new();
await eventStoreService.SubscribeToStreamAsync(
    streamName: $"customer-{accountCreatedEvent.CostumerId}-stream",
    async (StreamSubscription, resolvedEvent, cancellationToken) =>
    {
        string eventType = resolvedEvent.Event.EventType;
        object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Type.GetType(eventType));

        switch (@event)
        {
            case AccountCreatedEvent e:
                balanceInfo.AccountId = e.AccountId;
                balanceInfo.Balance = e.StartBalance;
                break;
            case MoneyDepositedEvent e:
                balanceInfo.Balance += e.Amount;
                break;
            case MoneyWithdrawnEvent e:
                balanceInfo.Balance -= e.Amount;
                break;
            case MoneyTransferredEvent e:
                balanceInfo.Balance -= e.Amount;
                break;
        }
        Console.WriteLine($"Balance : {JsonSerializer.Serialize(balanceInfo.Balance)}");
    });


Console.WriteLine("Added");
Console.ReadLine();












class BalanceInfo
{
    public string AccountId { get; set; }
    public int Balance { get; set; }
}
class AccountCreatedEvent
{
    public string AccountId { get; set; }
    public string CostumerId { get; set; }
    public int StartBalance { get; set; }
    public DateTime Date { get; set; }
}
class MoneyDepositedEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}
class MoneyWithdrawnEvent
{
    public string AccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}
class MoneyTransferredEvent
{
    public string AccountId { get; set; }
    public string TargetAccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}
