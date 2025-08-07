using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Domain.Models;

public class MbError(ProblemDetails problemDetails)
{
    /// <summary>
    /// Описание ошибки
    /// </summary>
    [UsedImplicitly]
    public ProblemDetails Error { get; set; } = problemDetails;
}

