using Lab5.Tools.Application.Models.ValueObjects;
using SourceKit.Generators.Builder.Annotations;

namespace Lab5.Tools.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record GetAccountQuery(AccountId[] Ids);