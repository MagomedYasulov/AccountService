
using JetBrains.Annotations;

namespace AccountService.Domain.Models;

public class MbResult<TValue>(TValue value)
{
    /// <summary>
    /// Запрашиваемые данные
    /// </summary>
    [UsedImplicitly]
    public TValue Value { get; set; } = value;
}