using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountService.Migrations
{
    /// <inheritdoc />
    public partial class AddAccrueInterestProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE PROCEDURE public.accrue_interest(p_account_id uuid)
LANGUAGE plpgsql
AS $$
DECLARE
    acc_record RECORD;
    v_amount numeric;
    v_now date := current_date;
    v_last_date date;
    v_days integer;
    c_deposit_type_const int := 1; 
    c_credit_tx_type_const int := 1;
BEGIN
    SELECT ""Id"", ""Type"", ""Balance"", ""InterestRate"", ""CurrencyCode"", ""LastAccruedAt"", ""OpenedAt""
    INTO acc_record
    FROM ""Accounts""
    WHERE ""Id"" = p_account_id
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Account % not found', p_account_id;
    END IF;

    IF acc_record.""Type"" IS DISTINCT FROM c_deposit_type_const THEN
        RAISE NOTICE 'Account % is not a deposit account, skipping', p_account_id;
        RETURN;
    END IF;

    IF acc_record.""InterestRate"" IS NULL THEN
        RAISE NOTICE 'Account % has no interest rate, skipping', p_account_id;
        RETURN;
    END IF;

    v_last_date := COALESCE(acc_record.""LastAccruedAt""::date, acc_record.""OpenedAt""::date);

    v_days := v_now - v_last_date;
    IF v_days <= 0 THEN
        RAISE NOTICE 'No days elapsed since last accrual for account %, skipping', p_account_id;
        RETURN;
    END IF;

    v_amount := acc_record.""Balance"" * (acc_record.""InterestRate"" / 100.0) * (v_days::numeric / 365.0);
    v_amount := round(v_amount::numeric, 2);

    IF v_amount <= 0 THEN
        RAISE NOTICE 'Accrued amount % for account % <= 0, skipping', v_amount, p_account_id;
        RETURN;
    END IF;

    INSERT INTO ""Transactions"" (
        ""Id"",
        ""AccountId"",
        ""CounterpartyAccountId"",
        ""Sum"",
        ""CurrencyCode"",
        ""Type"",
        ""Description"",
        ""TransferTime"",
        ""CreatedAt""
    )
    VALUES (
        gen_random_uuid(),
        p_account_id,
        NULL,
        v_amount,
        acc_record.""CurrencyCode"",
        c_credit_tx_type_const,
        'accrual of interest on a deposit',
        now(),
        now()
    );

    UPDATE ""Accounts""
    SET ""Balance"" = ""Balance"" + v_amount,
        ""LastAccruedAt"" = v_now
    WHERE ""Id"" = p_account_id;
END;
$$;

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS public.accrue_interest(uuid);");
        }
    }
}
