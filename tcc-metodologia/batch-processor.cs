using System.Diagnostics;

namespace TccGreenAi;

public sealed record BatchResult(
    IReadOnlyList<Message> Messages,
    TimeSpan Elapsed,
    TimeSpan ProcessorTime,
    long WorkingSetBeforeBytes,
    long WorkingSetAfterBytes,
    long ManagedHeapBeforeBytes,
    long ManagedHeapAfterBytes,
    int Repetitions,
    int Correct,
    int Incorrect)
{
    public double AccuracyPercent => Messages.Count == 0 ? 0 : Correct * 100.0 / Messages.Count;
    public long TotalClassifications => (long)Messages.Count * Repetitions;
    public double MessagesPerSecond => Elapsed.TotalSeconds <= 0 ? 0 : TotalClassifications / Elapsed.TotalSeconds;
}

public sealed class BatchProcessor(IMessageClassifier classifier)
{
    public BatchResult Process(IReadOnlyList<Message> messages, int repetitions = 1)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
            throw new ArgumentException("O lote não pode estar vazio.", nameof(messages));
        if (repetitions <= 0)
            throw new ArgumentOutOfRangeException(nameof(repetitions), "O número de repetições deve ser maior que zero.");

        using var process = System.Diagnostics.Process.GetCurrentProcess();
        process.Refresh();
        var processorBefore = process.TotalProcessorTime;
        var workingSetBefore = process.WorkingSet64;
        var managedHeapBefore = GC.GetTotalMemory(forceFullCollection: false);
        var stopwatch = Stopwatch.StartNew();
        var classified = new Message[messages.Count];
        var correct = 0;

        for (var repetition = 0; repetition < repetitions; repetition++)
        {
            correct = 0;
            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                var category = classifier.Classify(message.Text);
                classified[i] = message with { Category = category };
                if (category == message.ExpectedCategory)
                    correct++;
            }
        }

        stopwatch.Stop();
        process.Refresh();
        var processorTime = process.TotalProcessorTime - processorBefore;
        var workingSetAfter = process.WorkingSet64;
        var managedHeapAfter = GC.GetTotalMemory(forceFullCollection: false);
        return new BatchResult(
            classified,
            stopwatch.Elapsed,
            processorTime,
            workingSetBefore,
            workingSetAfter,
            managedHeapBefore,
            managedHeapAfter,
            repetitions,
            correct,
            messages.Count - correct);
    }
}
