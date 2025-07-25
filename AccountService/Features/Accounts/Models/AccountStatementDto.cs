namespace AccountService.Features.Accounts.Models
{
    public class AccountStatementDto
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public TransactionStatementDto[] Transactions { get; set; } = [];
    }
}
