
namespace TEST2
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
            this.Connect_button = new System.Windows.Forms.Button();
            this.MoveBed_button = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Connect_button
            // 
            this.Connect_button.Location = new System.Drawing.Point(12, 26);
            this.Connect_button.Name = "Connect_button";
            this.Connect_button.Size = new System.Drawing.Size(94, 28);
            this.Connect_button.TabIndex = 0;
            this.Connect_button.Text = "测试机械连接";
            this.Connect_button.UseVisualStyleBackColor = true;
            this.Connect_button.Click += new System.EventHandler(this.Connect_button_Click);
            // 
            // MoveBed_button
            // 
            this.MoveBed_button.Location = new System.Drawing.Point(140, 26);
            this.MoveBed_button.Name = "MoveBed_button";
            this.MoveBed_button.Size = new System.Drawing.Size(88, 28);
            this.MoveBed_button.TabIndex = 1;
            this.MoveBed_button.Text = "测试运动";
            this.MoveBed_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.MoveBed_button.UseVisualStyleBackColor = true;
            this.MoveBed_button.Click += new System.EventHandler(this.MoveBed_button_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(254, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 28);
            this.button1.TabIndex = 2;
            this.button1.Text = "测试数据库连接";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 79);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MoveBed_button);
            this.Controls.Add(this.Connect_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Connect_button;
        private System.Windows.Forms.Button MoveBed_button;
        private System.Windows.Forms.Button button1;
    }
}

