using Lab5.Tools.Application.Models.ValueObjects;

namespace Lab5.Tools.Application.Models;

public sealed record Account(AccountId Id, UserId UserId);