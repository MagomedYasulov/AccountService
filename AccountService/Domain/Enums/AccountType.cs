using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountService.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AccountType
{
    Checking,

    [UsedImplicitly]
    Deposit,

    [UsedImplicitly]
    Credit
}