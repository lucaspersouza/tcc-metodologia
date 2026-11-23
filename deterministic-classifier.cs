using System.Text;
using System.Text.RegularExpressions;

namespace TccGreenAi;

public interface IMessageClassifier
{
    string Name { get; }
    MessageCategory Classify(string text);
}

/// <summary>
/// Classificador explicável baseado em palavras-chave; não depende de serviços externos.
/// Os padrões de Regex são criados e compilados uma única vez, no construtor, e reutilizados em todas as classificações.
/// </summary>
public sealed class DeterministicClassifier : IMessageClassifier
{
    private static readonly (MessageCategory Category, string[] Keywords)[] Rules =
    [
        (MessageCategory.Suporte, ["erro", "falha", "bug", "travou", "problema", "ajuda", "parou de funcionar", "acessar"]),
        (MessageCategory.Vendas, ["preco", "comprar", "orcamento", "contratar", "licenca", "desconto", "plano"]),
        (MessageCategory.Financeiro, ["boleto", "fatura", "pagamento", "cobranca", "nota fiscal", "vencimento", "mensalidade"])
    ];

    private readonly Regex _nonSpacingMarks;
    private readonly (MessageCategory Category, Regex[] Patterns)[] _compiledRules;

    public DeterministicClassifier()
    {
        _nonSpacingMarks = new Regex(@"\p{Mn}", RegexOptions.Compiled);
        _compiledRules = Rules
            .Select(rule => (rule.Category, rule.Keywords
                .Select(keyword => new Regex($@"\b{Regex.Escape(keyword)}\b", RegexOptions.Compiled))
                .ToArray()))
            .ToArray();
    }

    public string Name => "Regras determinísticas";

    public MessageCategory Classify(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var normalized = Normalize(text);

        foreach (var (category, patterns) in _compiledRules)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(normalized))
                    return category;
            }
        }

        return MessageCategory.Outros;
    }

    private string Normalize(string value)
    {
        var decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return _nonSpacingMarks.Replace(decomposed, "").Normalize(NormalizationForm.FormC);
    }
}