﻿namespace AccountService.Domain.Models;

public class AccountFilter
{
    public Guid? OwnerId { get; set; }
    public bool? Revoked { get; set; }
}