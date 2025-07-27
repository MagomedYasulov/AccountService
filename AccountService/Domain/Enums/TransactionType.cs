using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountService.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionType
{
    Credit,
    Debit
}