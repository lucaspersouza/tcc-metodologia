using System.Diagnostics;
using System.Globalization;
using TccGreenAi;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

try
{
    var options = ParseArguments(args);
    var methodName = options.Method == "ai" ? "Com IA local (Naive Bayes)" : "Sem IA (regras)";
    var output = options.Output ?? Path.Combine("resultados", $"{options.Method}-mensagens.csv");
    var metrics = options.Metrics ?? Path.Combine("resultados", $"{options.Method}-metricas.csv");

    PrintMarker("EXPERIMENTO_INICIO", methodName);
    Console.WriteLine($"Mensagens fictícias: {options.Count:N0} | semente: {options.Seed} | repetições: {options.Repetitions}");
    var messages = MessageGenerator.Generate(options.Count, options.Seed);

    IMessageClassifier classifier;
    var trainingTime = TimeSpan.Zero;
    if (options.Method == "ai")
    {
        PrintMarker("TREINAMENTO_INICIO", "Naive Bayes com corpus sintético");
        var trainingTimer = Stopwatch.StartNew();
        classifier = new NaiveBayesClassifier(MessageGenerator.GenerateTrainingSet());
        trainingTimer.Stop();
        trainingTime = trainingTimer.Elapsed;
        PrintMarker("TREINAMENTO_FIM", $"{trainingTime.TotalMilliseconds:F3} ms");
    }
    else
    {
        classifier = new DeterministicClassifier();
        Console.WriteLine("Treinamento: não aplicável ao classificador baseado em regras.");
    }

    PrintMarker("CLASSIFICACAO_INICIO", methodName);
    var result = new BatchProcessor(classifier).Process(messages, options.Repetitions);
    PrintMarker("CLASSIFICACAO_FIM", methodName);

    await CsvReport.WriteMessagesAsync(output, result);
    await CsvReport.WriteMetricsAsync(metrics, methodName, result, trainingTime);

    Console.WriteLine($"Acertos: {result.Correct:N0} | erros: {result.Incorrect:N0} | acurácia na base sintética: {result.AccuracyPercent:F2}%");
    Console.WriteLine($"Tempo de classificação: {result.Elapsed.TotalMilliseconds:F2} ms | CPU do processo: {result.ProcessorTime.TotalMilliseconds:F2} ms");
    Console.WriteLine($"Vazão: {result.MessagesPerSecond:N0} classificações/s");
    Console.WriteLine($"Relatório por mensagem: {Path.GetFullPath(output)}");
    Console.WriteLine($"Métricas do processo: {Path.GetFullPath(metrics)}");
    Console.WriteLine("As métricas de hardware devem ser gravadas separadamente no HWiNFO usando os marcadores de horário acima.");
    PrintMarker("EXPERIMENTO_FIM", methodName);
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Parâmetro inválido: {exception.Message}");
    Console.Error.WriteLine("Uso: dotnet run -- --method rules|ai [--count 5000] [--seed 42] [--repetitions 20] [--output caminho.csv] [--metrics caminho.csv]");
    Environment.ExitCode = 1;
}

static void PrintMarker(string marker, string details) =>
    Console.WriteLine($"[{DateTimeOffset.Now:O}] {marker}: {details}");

static (string Method, int Count, int Seed, int Repetitions, string? Output, string? Metrics) ParseArguments(string[] arguments)
{
    string? method = null;
    int count = 5000;
    int seed = 42;
    int repetitions = 20;
    string? output = null;
    string? metrics = null;

    for (var i = 0; i < arguments.Length; i++)
    {
        if (i + 1 >= arguments.Length)
            throw new ArgumentException($"Faltou um valor para {arguments[i]}.");

        var option = arguments[i];
        var value = arguments[++i];
        switch (option)
        {
            case "--method" when value is "rules" or "ai":
                method = value;
                break;
            case "--count" when int.TryParse(value, out var parsedCount) && parsedCount > 0:
                count = parsedCount;
                break;
            case "--seed" when int.TryParse(value, out var parsedSeed):
                seed = parsedSeed;
                break;
            case "--repetitions" when int.TryParse(value, out var parsedRepetitions) && parsedRepetitions > 0:
                repetitions = parsedRepetitions;
                break;
            case "--output" when !string.IsNullOrWhiteSpace(value):
                output = value;
                break;
            case "--metrics" when !string.IsNullOrWhiteSpace(value):
                metrics = value;
                break;
            default:
                throw new ArgumentException($"Opção ou valor inválido: {option} {value}");
        }
    }

    if (method is null)
        throw new ArgumentException("Escolha o sistema com --method rules ou --method ai.");

    return (method, count, seed, repetitions, output, metrics);
}
