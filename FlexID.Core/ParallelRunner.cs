namespace FlexID;

public class ParallelRunner<T>
{
    public ParallelRunner(IEnumerable<T> items)
    {
        Items = items.ToArray();
    }

    public IReadOnlyList<T> Items { get; }

    public int ParallelCount { get; set; } = -1;

    public delegate void ItemNotifyEventHandler(T item);
    public delegate void ItemFailureEventHandler(T item, Exception exception);

    public event ItemNotifyEventHandler? StartItem;
    public event ItemNotifyEventHandler? SuccessItem;
    public event ItemFailureEventHandler? FailureItem;

    public async Task StartAsync(Action<T, CancellationToken> action, CancellationToken cancellationToken)
    {
        var parallelCount = ParallelCount;
        if (parallelCount == 0)
            throw new ArgumentOutOfRangeException(nameof(ParallelCount), "ParallelCount should be -1 or a positive integer.");

        if (parallelCount < 0)
            parallelCount = Environment.ProcessorCount + parallelCount;
        parallelCount = Math.Max(parallelCount, 1);
        parallelCount = Math.Min(parallelCount, Environment.ProcessorCount);

        var semaphore = new SemaphoreSlim(parallelCount);

        await Task.WhenAll(Items.Select(async item =>
        {
            // 同時に起動・実行されるタスク数を制限する。
            await semaphore.WaitAsync(cancellationToken);

            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    StartItem?.Invoke(item);
                    action(item, cancellationToken);
                    SuccessItem?.Invoke(item);
                }
                catch (Exception exception)
                {
                    // 何らかのエラーが発生した場合。
                    FailureItem?.Invoke(item, exception);
                }

                // 1ケースの計算が終了するごとにGCを呼ばないと、並列計算数がだんだん減ってしまう。
                GC.Collect();

            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
            .ContinueWith(_ => semaphore.Release());
        }));
    }
}
