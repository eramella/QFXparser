# QFXparser
Simple parser for .qfx and .qbo files.

Pass to the library a file path or a Strem and it will return a "Statement" object that contains the Account number and the list of Transactions.

Example:

```csharp
using QFXparser;

FileParser parser = new FileParser("C:\\filename.qbo");
Statement result = parser.BuildStatement();
```

The returned objects are:

**Statement**
```csharp
public class Statement
{
    public string AccountNum { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

**Transaction**
```csharp
public class Transaction
{
    public string Type { get; set; }
    public DateTime PostedOn { get; set; }
    public Decimal Amount { get; set; }
    public string TransactionId { get; set; }
    public string RefNumber { get; set; }
    public string Name { get; set; }
    public string Memo { get; set; }
}
```

### Transaction Dates
In case the financial institution is not including the timezone with the transaction date, we assume it is UTC.


Please let me know if any issues and if you like give a star.