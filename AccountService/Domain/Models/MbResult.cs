using Microsoft.AspNetCore.Mvc;

namespace AccountService.Domain.Models;

public class MbResult<TValue>
{
    /// <summary>
    /// Запрашиваемые данные
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// Описание ошибки
    /// </summary>
    public ProblemDetails? Error { get; set; }


    public MbResult() { }

    public MbResult(TValue value)
    {
        Value = value;
    }
}

public class MbResult : MbResult<DefaultType>;

public class DefaultType;