namespace DownMovies
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.pgPage = new System.Windows.Forms.ProgressBar();
            this.pgDown = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lvMovies = new System.Windows.Forms.ListView();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblPgSt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(99, 75);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // pgPage
            // 
            this.pgPage.Location = new System.Drawing.Point(134, 32);
            this.pgPage.Name = "pgPage";
            this.pgPage.Size = new System.Drawing.Size(407, 12);
            this.pgPage.TabIndex = 1;
            // 
            // pgDown
            // 
            this.pgDown.Location = new System.Drawing.Point(134, 76);
            this.pgDown.Name = "pgDown";
            this.pgDown.Size = new System.Drawing.Size(407, 11);
            this.pgDown.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "抓取页面进度";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "抓取下载地址进度";
            // 
            // lvMovies
            // 
            this.lvMovies.Location = new System.Drawing.Point(12, 109);
            this.lvMovies.Name = "lvMovies";
            this.lvMovies.Size = new System.Drawing.Size(726, 351);
            this.lvMovies.TabIndex = 3;
            this.lvMovies.UseCompatibleStateImageBehavior = false;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(553, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(185, 91);
            this.txtLog.TabIndex = 4;
            // 
            // lblPgSt
            // 
            this.lblPgSt.AutoSize = true;
            this.lblPgSt.ForeColor = System.Drawing.Color.Maroon;
            this.lblPgSt.Location = new System.Drawing.Point(245, 9);
            this.lblPgSt.Name = "lblPgSt";
            this.lblPgSt.Size = new System.Drawing.Size(0, 12);
            this.lblPgSt.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 472);
            this.Controls.Add(this.lblPgSt);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lvMovies);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pgDown);
            this.Controls.Add(this.pgPage);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ProgressBar pgPage;
        private System.Windows.Forms.ProgressBar pgDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvMovies;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblPgSt;
    }
}

