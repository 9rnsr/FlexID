#define DEBUG_ParallelRunningCount

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ResultChecker
{
    class ProgressPresenter
    {
        private class ItemData
        {
            public string Name { get; }

            public string Status { get; private set; }

            public string Message { get; private set; }

            public bool IsFinished { get; private set; }

            public ItemData(string name)
            {
                Name = name;
            }

            public void SetStatus(string status, string message)
            {
                Status = status;
                Message = message;
                IsFinished = true;
            }
        }

        private readonly Task task;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly List<ItemData> items = new List<ItemData>();
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        private string windmill = "/";
        private readonly TimeSpan wait = TimeSpan.FromMilliseconds(250);

        private int totalCount;
        private int finishCount;
        private const int DumpCount = 10;
#if DEBUG_ParallelRunningCount
        private int runningCount;
#endif

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="totalCount">項目の総数。</param>
        public ProgressPresenter(int totalCount)
        {
            this.totalCount = totalCount;
            this.finishCount = 0;

            var cancellationToken = cancellationTokenSource.Token;

            // 一定間隔ごとに画面を更新するためのタスク。
            task = new Task(async () =>
            {
                // カーソルを非表示にする。
                Console.Write("\x1B[?25l");

                DumpOut();

                while (!cancellationToken.IsCancellationRequested)
                {
                    Update();
                    try
                    {
                        await Task.Delay(wait, cancellationToken);
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

            }, TaskCreationOptions.LongRunning);

            task.Start();
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
                items.Add(item);
                PrintOut(item, overwriteLine: true);
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
        public void Stop(string name, string status, string message = null)
        {
            locker.EnterReadLock();
            try
            {
                var i = items.IndexOf(item => item.Name == name);
                if (i == -1)
                    return;

                var target = items[i];
                target.SetStatus(status, message);
#if DEBUG_ParallelRunningCount
                System.Diagnostics.Debug.WriteLine($"END   {name} : ThreadID = {Thread.CurrentThread.ManagedThreadId}");
#endif
            }
            finally
            {
                locker.ExitReadLock();
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
                if (items.Count == 0)
                    return;

                // カーソルをitems.Count行上の先頭に移動。
                Console.Write($"\x1B[{items.Count}F");

                var overwriteLine = false;
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

        public async Task WaitForExit()
        {
            cancellationTokenSource.Cancel();

            await task;
        }
    }
}
