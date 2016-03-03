using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            //捕捉窗口关闭事件
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


        async void GrabDetailPages(CancellationToken token)
        {
            AppendLog("[页面列表] 正在加载数据....");
            //从第一页开始
            var page = 1;
            var urlformat = "http://www.dy2018.com/html/gndy/dyzz/index_{0}.html";
            //网络客户端
            var client = new HttpClient();
            var data = TaskContext.Instance.Data;
            while (!token.IsCancellationRequested)
            {
                AppendLog("[页面列表] 正在加载第 {0} 页", page);
                var url = page == 1 ? "http://www.dy2018.com/html/gndy/dyzz/index.html" : urlformat.FormatWith(page);
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
                            .Select(s => new PageTask(Convert.ToInt32(Regex.Match(s.Groups[1].Value, @"(\d+)").Value.ToString()), s.Groups[2].Value, s.Groups[1].Value))
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
                    if (!Regex.IsMatch(ctx.Result, @"<a[^>]*?href=['""][^>]*?下一页</a[^>]", RegexOptions.IgnoreCase))
                    {
                        AppendLog("[页面列表] 没有更多的页面，退出抓取...");
                        data.FullDownloaded = true;
                        break;
                    }
                    //如果超过200页则终止
                    if (page > 200)
                    {
                        AppendLog("[页面列表] 超过200页，退出抓取...");
                        break;
                    }
                    //等待2秒继续
                    await Task.Delay(new TimeSpan(0, 0, 2));
                    page++;
                }
                //更新数据到listview
                //UpdateLv();
            }
            AppendLog("[页面列表] 页面任务抓取完成...");
            Invoke(new Action(() => btnStart.Enabled = true));
        }

        private void UpdateLv()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateLv));
            }
        }

        private void UpdatePageDetailGrabStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdatePageDetailGrabStatus));
            }
            var data = TaskContext.Instance.Data;
            pgPage.Maximum = data.WaitForDownloadPageTasks.Count + data.PageDownloaded.Count;
            pgPage.Value = pgPage.Maximum - data.WaitForDownloadPageTasks.Count;
            lblPgSt.Text = $"共 {pgPage.Maximum} 页面，已抓取 {pgPage.Value} 页面 ...";
        }

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
    }
}
