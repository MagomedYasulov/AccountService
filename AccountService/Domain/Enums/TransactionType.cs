using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountService.Domain.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionType
{
    /// <summary>
    /// Зачисление
    /// </summary>
    Credit,

    /// <summary>
    /// Снятие
    /// </summary>
    Debit
}