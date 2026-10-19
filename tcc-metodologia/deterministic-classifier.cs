using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TccGreenAi;

public interface IMessageClassifier
{
    string Name { get; }
    MessageCategory Classify(string text);
}

/// <summary>Classificador explicável baseado em palavras-chave; não depende de serviços externos.</summary>
public sealed class DeterministicClassifier : IMessageClassifier
{
    private static readonly Regex NonSpacingMarks = new(@"\p{Mn}", RegexOptions.Compiled);
    private static readonly (MessageCategory Category, string[] Keywords)[] Rules =
    [
        (MessageCategory.Suporte, ["erro", "falha", "bug", "travou", "problema", "ajuda", "parou de funcionar", "acessar"]),
        (MessageCategory.Vendas, ["preco", "comprar", "orcamento", "contratar", "licenca", "desconto", "plano"]),
        (MessageCategory.Financeiro, ["boleto", "fatura", "pagamento", "cobranca", "nota fiscal", "vencimento", "mensalidade"])
    ];

    public string Name => "Regras determinísticas";

    public MessageCategory Classify(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var normalized = Normalize(text);

        foreach (var (category, keywords) in Rules)
        {
            if (keywords.Any(keyword => Regex.IsMatch(normalized, $@"\b{Regex.Escape(keyword)}\b")))
                return category;
        }

        return MessageCategory.Outros;
    }

    private static string Normalize(string value)
    {
        var decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return NonSpacingMarks.Replace(decomposed, "").Normalize(NormalizationForm.FormC);
    }
}
