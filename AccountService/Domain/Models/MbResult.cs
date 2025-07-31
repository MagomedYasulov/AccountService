using Microsoft.AspNetCore.Mvc;

namespace AccountService.Domain.Models
{
    public class MbResult<TValue>
    {
        public TValue? Value { get; set; }
        public ProblemDetails? Error { get; set; }


        public MbResult() { }

        public MbResult(TValue value)
        {
            Value = value;
        }
    }

    public class MbResult : MbResult<DefaultType> { }

    public class DefaultType { }
}
