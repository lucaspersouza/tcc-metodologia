using System.Text;
using System.Text.RegularExpressions;

namespace TccGreenAi;

/// <summary>Classificador local de aprendizado de máquina baseado em Naive Bayes multinomial.</summary>
public sealed class NaiveBayesClassifier : IMessageClassifier
{
    private static readonly Regex TokenPattern = new("[a-z0-9]+", RegexOptions.Compiled);
    private static readonly HashSet<string> StopWords = [
        "a", "ao", "aos", "as", "com", "da", "das", "de", "do", "dos", "e", "em", "é", "o", "os",
        "para", "por", "qual", "que", "um", "uma", "no", "na", "nos", "nas", "meu", "minha", "meus", "minhas",
        "eu", "como", "está", "estao", "foi", "não", "nao", "já", "ja", "mas", "vocês", "voces"
    ];
    private readonly MessageCategory[] _categories;
    private readonly Dictionary<MessageCategory, Dictionary<string, int>> _wordCounts = [];
    private readonly Dictionary<MessageCategory, int> _messageCounts = [];
    private readonly HashSet<string> _vocabulary = [];
    private int _totalMessages;

    public NaiveBayesClassifier(IEnumerable<Message> trainingMessages)
    {
        ArgumentNullException.ThrowIfNull(trainingMessages);
        _categories = Enum.GetValues<MessageCategory>();

        foreach (var category in _categories)
        {
            _wordCounts[category] = [];
            _messageCounts[category] = 0;
        }

        foreach (var message in trainingMessages)
        {
            _messageCounts[message.ExpectedCategory]++;
            _totalMessages++;

            foreach (var token in Tokenize(message.Text))
            {
                _vocabulary.Add(token);
                var counts = _wordCounts[message.ExpectedCategory];
                counts[token] = counts.GetValueOrDefault(token) + 1;
            }
        }

        if (_totalMessages == 0 || _vocabulary.Count == 0)
            throw new ArgumentException("O conjunto de treinamento não pode estar vazio.", nameof(trainingMessages));
    }

    public string Name => "IA local (Naive Bayes)";

    public MessageCategory Classify(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        var tokens = Tokenize(text);
        var bestCategory = _categories[0];
        var bestScore = double.NegativeInfinity;

        foreach (var category in _categories)
        {
            var categoryMessageCount = _messageCounts[category];
            if (categoryMessageCount == 0)
                continue;

            var counts = _wordCounts[category];
            var totalWords = counts.Values.Sum();
            var denominator = totalWords + _vocabulary.Count;
            var score = Math.Log((double)categoryMessageCount / _totalMessages);

            foreach (var token in tokens)
                score += Math.Log((counts.GetValueOrDefault(token) + 1.0) / denominator);

            if (score > bestScore)
            {
                bestScore = score;
                bestCategory = category;
            }
        }

        return bestCategory;
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        var normalized = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var withoutAccents = Regex.Replace(normalized, @"\p{Mn}", "").Normalize(NormalizationForm.FormC);
        return TokenPattern.Matches(withoutAccents)
            .Select(match => match.Value)
            .Where(token => !StopWords.Contains(token));
    }
}
