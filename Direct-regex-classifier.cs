using System.Text;
using System.Text.RegularExpressions;

namespace TccGreenAi;

/// <summary>
/// Variante de controle: mesma lógica e mesmas palavras-chave de <see cref="DeterministicClassifier"/>,
/// mas na implementação direta original, em que o padrão de Regex é montado a cada chamada,
/// sem ser pré-compilado nem reutilizado. Serve para medir quanto o custo energético depende desse detalhe de implementação.
/// </summary>
public sealed class DirectRegexClassifier : IMessageClassifier
{
    private static readonly Regex NonSpacingMarks = new(@"\p{Mn}", RegexOptions.Compiled);
    private static readonly (MessageCategory Category, string[] Keywords)[] Rules =
    [
        (MessageCategory.Suporte, ["erro", "falha", "bug", "travou", "problema", "ajuda", "parou de funcionar", "acessar"]),
        (MessageCategory.Vendas, ["preco", "comprar", "orcamento", "contratar", "licenca", "desconto", "plano"]),
        (MessageCategory.Financeiro, ["boleto", "fatura", "pagamento", "cobranca", "nota fiscal", "vencimento", "mensalidade"])
    ];

    public string Name => "Regras determinísticas (implementação direta)";

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