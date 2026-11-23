namespace TccGreenAi;

public enum MessageCategory
{
    Suporte,
    Vendas,
    Financeiro,
    Outros
}

public sealed record Message(
    int Id,
    string Text,
    MessageCategory ExpectedCategory,
    MessageCategory? Category = null);
