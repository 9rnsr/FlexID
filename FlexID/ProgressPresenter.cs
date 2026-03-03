#define DEBUG_ParallelRunningCount

namespace FlexID;

class ProgressPresenter
{
    private class ItemData
    {
        public string Name { get; }

        public string Status { get; private set; } = "";

        public string? Message { get; private set; }

        public bool IsFinished { get; private set; }

        public ItemData(string name)
        {
            Name = name;
        }

        public void SetStatus(string status, string? message)
        {
            Status = status;
            Message = message;
            IsFinished = true;
        }
    }

    private readonly Task task;
    private readonly CancellationTokenSource cts = new();

    private readonly List<ItemData> items = [];
    private readonly List<ItemData> newItems = [];
    private readonly ReaderWriterLockSlim locker = new();

    private string windmill = "/";
    private readonly TimeSpan wait = TimeSpan.FromMilliseconds(250);

    private readonly int totalCount;
    private int finishCount;
    private const int DumpCount = 10;
#if DEBUG_ParallelRunningCount
    private int runningCount;
#endif

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="totalCount">項目の総数。</param>
    /// <param name="cancellationToken"></param>
    public ProgressPresenter(int totalCount, CancellationToken cancellationToken)
    {
        this.totalCount = totalCount;
        this.finishCount = 0;

        var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        // 一定間隔ごとに画面を更新するためのタスク。
        task = Task.Run(async () =>
        {
            // カーソルを非表示にする。
            Console.Write("\x1B[?25l");

            DumpOut();

            while (!ctsLinked.IsCancellationRequested)
            {
                Update();
                try
                {
                    await Task.Delay(wait, ctsLinked.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
            Update();

            if (totalCount % DumpCount != 0)
                DumpOut();

            // カーソルを再表示する。
            Console.Write("\x1B[?25h");

        }, cancellationToken);
    }

    /// <summary>
    /// 実行開始した新しい項目を追加する。
    /// </summary>
    /// <param name="name">項目名。</param>
    public void Start(string name)
    {
        locker.EnterWriteLock();
        try
        {
            var item = new ItemData(name);
            newItems.Add(item);
#if DEBUG_ParallelRunningCount
            //System.Diagnostics.Debug.WriteLine($"START {name} : ThreadID = {Thread.CurrentThread.ManagedThreadId}");
#endif
        }
        finally
        {
            locker.ExitWriteLock();
        }
    }

    /// <summary>
    /// 項目を実行終了したうえで最終状態を設定する。
    /// </summary>
    /// <param name="name">項目名。</param>
    /// <param name="status">最終状態。</param>
    /// <param name="message">付加的なメッセージ。</param>
    public void Stop(string name, string status, string? message = null)
    {
        locker.EnterWriteLock();
        try
        {
            var target = items.Concat(newItems).FirstOrDefault(item => item.Name == name);
            if (target is null)
                return;

            target.SetStatus(status, message);
#if DEBUG_ParallelRunningCount
            System.Diagnostics.Debug.WriteLine($"END   {name} : ThreadID = {Thread.CurrentThread.ManagedThreadId}");
#endif
        }
        finally
        {
            locker.ExitWriteLock();
        }
    }

    /// <summary>
    /// 画面の更新処理。
    /// </summary>
    private void Update()
    {
        switch (windmill)
        {
            case @"/": windmill = @"-"; break;
            case @"-": windmill = @"\"; break;
            case @"\": windmill = @"|"; break;
            case @"|": windmill = @"/"; break;
        }

        locker.EnterWriteLock();
        try
        {
            //System.Diagnostics.Debug.WriteLine($"# items.Count = {items.Count}, newItems.Count = {newItems.Count}");
            if (items.Count == 0 && newItems.Count == 0)
                return;

            // カーソルをitems.Count行上の先頭に移動。
            if (items.Count != 0)
                Console.Write($"\x1B[{items.Count}F");

            // 行全体を更新するかどうか。新しく追加された、あるいは終了状態に移行した項目が
            // ある場合は、当該項目以降の全ての行を書き換える必要があるためtrueになる。
            var overwriteLine = newItems.Count != 0;

            // 新しく追加され、まだ未表示の項目をitemsに追加する。
            items.AddRange(newItems);
            newItems.Clear();

            // 終了した項目を出力。
            foreach (var item in items.Where(item => item.IsFinished))
            {
                overwriteLine = true;
                PrintOut(item, overwriteLine);
                if ((++finishCount % DumpCount) == 0)
                    DumpOut();
            }

            // 終了した項目を除去。
            items.RemoveAll(item => item.IsFinished);

            // 実行中の項目を出力。
            foreach (var item in items)
                PrintOut(item, overwriteLine);

#if DEBUG_ParallelRunningCount
            if (items.Count != runningCount)
            {
                runningCount = items.Count;
                System.Diagnostics.Debug.WriteLine($"runningCount = {runningCount}");
            }
#endif
        }
        finally
        {
            locker.ExitWriteLock();
        }
    }

    /// <summary>
    /// 項目情報の出力処理。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="overwriteLine">行全体を書き換える場合は<see langword="true"/>。</param>
    void PrintOut(ItemData item, bool overwriteLine)
    {
        if (item.IsFinished)
            overwriteLine = true;

        if (!overwriteLine)
        {
            // 風車部分のみ更新。
            Console.WriteLine($"\x1B[1C\x1B[33m{windmill}\x1B[0m");
            return;
        }

        // 行末まで消去。
        Console.Write("\x1B[K");

        // 終了状態または風車を出力。
        Console.Write("[");
        if (item.IsFinished)
            Console.Write(item.Status);
        else
            Console.Write($"\x1B[33m{windmill}\x1B[0m");
        Console.Write("] ");

        // 項目名を出力。
        Console.Write(item.Name);

        // メッセージがある場合はこれを追加出力。
        if (item.IsFinished && item.Message != null)
            Console.WriteLine($" : \x1B[33m{item.Message}\x1B[0m");
        else
            Console.WriteLine();
    }

    /// <summary>
    /// ダンプ情報の出力処理。
    /// </summary>
    private void DumpOut()
    {
        Console.Write("\x1B[K");
        Console.WriteLine($"=== {finishCount} / {totalCount} done ===");
#if DEBUG_ParallelRunningCount
        System.Diagnostics.Debug.WriteLine($"=== {finishCount} / {totalCount} done ===");
#endif
    }

    /// <summary>
    /// 表示更新処理の終了を待機する
    /// </summary>
    /// <returns></returns>
    public async Task WaitForExit()
    {
        cts.Cancel();

        await task;
    }
}
