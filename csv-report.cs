using System.Text;

namespace TccGreenAi;

public static class CsvReport
{
    public static async Task WriteMessagesAsync(string path, BatchResult result, CancellationToken cancellationToken = default)
    {
        var fullPath = PreparePath(path);
        await using var writer = CreateWriter(fullPath);
        await writer.WriteLineAsync("Id;Mensagem;AssuntoEsperado;AssuntoClassificado;Acertou".AsMemory(), cancellationToken);

        foreach (var message in result.Messages)
        {
            var line = string.Join(";",
                message.Id.ToString(), Escape(message.Text), message.ExpectedCategory.ToString(),
                message.Category?.ToString() ?? "", (message.Category == message.ExpectedCategory).ToString());
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
    }

    public static async Task WriteMetricsAsync(
        string path,
        string method,
        BatchResult result,
        TimeSpan trainingTime,
        CancellationToken cancellationToken = default)
    {
        var fullPath = PreparePath(path);
        await using var writer = CreateWriter(fullPath);
        await writer.WriteLineAsync("Metodo;Repeticoes;ClassificacoesTotais;TreinamentoMs;ClassificacaoMs;CpuProcessoMs;WorkingSetAntesMB;WorkingSetDepoisMB;HeapGerenciadoAntesMB;HeapGerenciadoDepoisMB;AcertosUltimoLote;ErrosUltimoLote;AcuraciaUltimoLote;MensagensPorSegundo".AsMemory(), cancellationToken);

        var row = string.Join(";",
            Escape(method), result.Repetitions.ToString(), result.TotalClassifications.ToString(),
            trainingTime.TotalMilliseconds.ToString("F3"), result.Elapsed.TotalMilliseconds.ToString("F3"),
            result.ProcessorTime.TotalMilliseconds.ToString("F3"),
            ToMegabytes(result.WorkingSetBeforeBytes), ToMegabytes(result.WorkingSetAfterBytes),
            ToMegabytes(result.ManagedHeapBeforeBytes), ToMegabytes(result.ManagedHeapAfterBytes),
            result.Correct.ToString(), result.Incorrect.ToString(), result.AccuracyPercent.ToString("F2"),
            result.MessagesPerSecond.ToString("F2"));
        await writer.WriteLineAsync(row.AsMemory(), cancellationToken);
    }

    private static string PreparePath(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        return fullPath;
    }

    private static StreamWriter CreateWriter(string path) =>
        new(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

    private static string ToMegabytes(long bytes) => (bytes / 1024.0 / 1024.0).ToString("F3");
    private static string Escape(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
