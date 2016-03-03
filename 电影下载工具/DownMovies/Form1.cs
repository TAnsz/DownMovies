using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FSLib.Network.Http;

namespace DownMovies
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnStart.Click += (s, e) =>
            {
                btnStart.Enabled = false;
                RunTask();
            };
        }

        private bool _shutdownFlag = false;
        private void RunTask()
        {
            btnStart.Enabled = false;
            var cts = new CancellationTokenSource();
            AppendLog("[全局]正在初始化...");
            TaskContext.Instance.Init();
            AppendLog("[全局]初始化完成...");

            AppendLog("[全局] 启动详情页下载任务...");
            var pageDownloadTask = new Task(() => GrabDetailPages(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
            pageDownloadTask.Start();
            AppendLog("[全局] 详情页抓取任务已启动...");

            AppendLog("[全局] 启动下载地址抓取任务...");
            var movieDownloadTask = new Task(() => GrabMovieListTaskThreadEntry(cts.Token), cts.Token, TaskCreationOptions.LongRunning);
            movieDownloadTask.Start();
            AppendLog("[全局] 下载地址抓取任务已启动...");
            //            捕捉窗口关闭事件
            //主要是给一个机会等待任务完成并把任务数据都保存
            FormClosing += async (s, e) =>
            {
                if (_shutdownFlag)
                    return;

                e.Cancel = !_shutdownFlag;
                AppendLog("[全局] 等待任务结束...");
                cts.Cancel();
                try
                {
                    await pageDownloadTask;
                }
                catch (Exception)
                {
                    // ignored
                }
                _shutdownFlag = true;
                TaskContext.Instance.Save();
                Close();
            };

        }

        #region 抓取电影列表页
        async void GrabDetailPages(CancellationToken token)
        {
            AppendLog("[页面列表] 正在加载数据....");
            //从第一页开始
            var page = 1;
            //最大抓取20页就足够了
            var maxpage = 20;
            //网络客户端
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;
            var site = "http://www.dy2018.com/";
            var url = site + "html/gndy/dyzz/index.html";
            while (!token.IsCancellationRequested)
            {
                AppendLog("[页面列表] 正在加载第 {0} 页", page);
                var ctx = client.Create<string>(HttpMethod.Get, url);
                await ctx.SendTask();
                if (!ctx.IsValid())
                {
                    AppendLog("[页面列表] 第 {0} 页下载失败，稍后重试", page);
                    await Task.Delay(new TimeSpan(0, 0, 10));
                }
                else
                {
                    //下载成功，获取列表
                    var matches = Regex.Matches(ctx.Result, @"<b>\s*?<a\shref=['""]([^['""]+)['""]\sclass=[""']ulink['""]\stitle=['""]([^['""]+)['""][^>]*?", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    var newTasks =
                        matches.Cast<Match>()
                            .Select(s => new PageTask(Convert.ToInt32(Regex.Match(s.Groups[1].Value, @"(\d+)").Value.ToString()), s.Groups[2].Value,
                         s.Groups[1].Value.IndexOf(site, StringComparison.Ordinal) >= 0
                                     ? s.Groups[1].Value
                                     : site + s.Groups[1].Value))
                            .Where(
                                s =>
                                    !data.PageDownloaded.ContainsKey(s.Url) &&
                                    data.WaitForDownloadPageTasks.All(x => x.Url != s.Url))
                            .ToArray();
                    if (newTasks.Length > 0)
                    {
                        lock (data.WaitForDownloadPageTasks)
                        {
                            newTasks.ForEach(s =>
                            {
                                data.WaitForDownloadPageTasks.Enqueue(s);
                            });
                        }
                        AppendLog("[页面列表] 已建立 {0} 新任务到队列中...", newTasks.Length);
                        UpdatePageDetailGrabStatus();
                    }
                    else if (data.FullDownloaded)
                    {
                        AppendLog("[页面列表]没有更多的新纪录，提出抓取...");
                        break;
                    }
                    //如果没有下一页，则终止
                    var next = Regex.Match(ctx.Result, @"<a\shref=['""]([^['""]+)['""]>下一页</a>", RegexOptions.IgnoreCase).Groups[1].ToString();
                    if (next.IsNullOrEmpty())
                    {
                        AppendLog("[页面列表] 没有更多的页面，退出抓取...");
                        data.FullDownloaded = true;
                        break;
                    }
                    url = site + next;
                    //如果超过最大页则终止
                    if (page > maxpage)
                    {
                        AppendLog("[页面列表] 超过{0}页，退出抓取...", maxpage);
                        data.FullDownloaded = true;
                        break;
                    }
                    //等待2秒继续
                    await Task.Delay(new TimeSpan(0, 0, 1));
                    page++;
                }
                //更新数据到listview
                //UpdateLv();
            }
            AppendLog("[页面列表] 页面任务抓取完成...");
            Invoke(new Action(() => btnStart.Enabled = true));
        }

        #region 更新列表页进度
        /// <summary>
        /// 更新列表页进度
        /// </summary>
        private void UpdatePageDetailGrabStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdatePageDetailGrabStatus));
            }
            var data = TaskContext.Instance.Data;
            pgPage.Maximum = data.WaitForDownloadPageTasks.Count + data.PageDownloaded.Count;
            pgPage.Value = pgPage.Maximum - data.WaitForDownloadPageTasks.Count;
            lblPgSt.Text = $"共 {pgPage.Maximum} 个电影，已抓取 {pgPage.Value} 电影下载地址 ...";
        }
        #endregion

        #endregion

        #region 抓取电影详情页

        void GrabMovieListTaskThreadEntry(CancellationToken token)
        {
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;
            PageTask currentTask;
            while (!token.IsCancellationRequested)
            {
                currentTask = null;
                //对队列进行加锁，防止详情页爬虫意外修改队列
                lock (data.WaitForDownloadPageTasks)
                {
                    //如果有队伍则出队
                    if (data.WaitForDownloadPageTasks.Count > 0)
                    {
                        currentTask = data.WaitForDownloadPageTasks.Dequeue();
                    }
                }
                //如果没有任务，则等待100ms后继续查询任务
                if (currentTask == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                AppendLog("[详情页抓取]正在抓取页面【{0}】...", currentTask.Name);

                currentTask.Root = currentTask.Name.GetSubString(40);
                //创建上下文。注意 allowAutoRedirect，因为这里可能会存在重定向，而我们并不关心不是302.
                var ctx = client.Create<string>(HttpMethod.Get, currentTask.Url, allowAutoRedirect: true);
                //同步模式
                ctx.Send();
                if (ctx.IsValid())
                {
                    //页面有效
                    var htm = ctx.Result;
                    //读取电影相关信息
                    var movieUrl = Regex.Match(htm, @"<a\shref=['""]([ftp]+[^'""]+)['""]>").Groups[1].Value;
                    if (!movieUrl.IsNullOrEmpty())
                    {
                        currentTask.DownloadUrl = movieUrl;
                        data.PageDownloaded.Add(currentTask.Url, currentTask);
                        AppendLog("[详情页抓取] 从页面 【{0}】中获得【{1}】下载地址 ...", currentTask.Url, currentTask.Name);
                        UpdateMovieDownloadStatus();
                    }
                }
            }
        }

        /// <summary>
        /// 更新页面抓取进度
        /// </summary>
        void UpdateMovieDownloadStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateMovieDownloadStatus));
                return;
            }

            var data = TaskContext.Instance.Data;
            pgPage.Value = pgPage.Maximum - data.WaitForDownloadPageTasks.Count;
            lblPgSt.Text = $"共 {pgPage.Maximum} 个电影，已抓取 {pgPage.Value} 电影下载地址 ...";
        }
        #endregion

        private void UpdateLv()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateLv));
            }
        }

        #region 添加日志
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        void AppendLog(string msg, params object[] args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { AppendLog(msg, args); }));
                return;
            }
            if (args == null || args.Length == 0)
            {
                txtLog.AppendText(msg);
            }
            else
            {
                txtLog.AppendText(string.Format(msg, args));
            }
            txtLog.AppendText(Environment.NewLine);
            txtLog.ScrollToCaret();
        }
        #endregion
    }
}
