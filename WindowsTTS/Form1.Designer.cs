namespace WindowsTTS
{
    partial class FormTTS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bt_play_pause = new System.Windows.Forms.Button();
            this.tb_content = new System.Windows.Forms.TextBox();
            this.bt_stop = new System.Windows.Forms.Button();
            this.bt_save = new System.Windows.Forms.Button();
            this.pb_tts = new System.Windows.Forms.ProgressBar();
            this.bt_open = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_play_pause
            // 
            this.bt_play_pause.Location = new System.Drawing.Point(108, 372);
            this.bt_play_pause.Name = "bt_play_pause";
            this.bt_play_pause.Size = new System.Drawing.Size(75, 23);
            this.bt_play_pause.TabIndex = 0;
            this.bt_play_pause.Text = "开始";
            this.bt_play_pause.UseVisualStyleBackColor = true;
            this.bt_play_pause.Click += new System.EventHandler(this.bt_play_pause_Click);
            // 
            // tb_content
            // 
            this.tb_content.Location = new System.Drawing.Point(12, 12);
            this.tb_content.Multiline = true;
            this.tb_content.Name = "tb_content";
            this.tb_content.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_content.Size = new System.Drawing.Size(363, 325);
            this.tb_content.TabIndex = 1;
            // 
            // bt_stop
            // 
            this.bt_stop.Location = new System.Drawing.Point(204, 372);
            this.bt_stop.Name = "bt_stop";
            this.bt_stop.Size = new System.Drawing.Size(75, 23);
            this.bt_stop.TabIndex = 2;
            this.bt_stop.Text = "停止";
            this.bt_stop.UseVisualStyleBackColor = true;
            this.bt_stop.Click += new System.EventHandler(this.bt_stop_Click);
            // 
            // bt_save
            // 
            this.bt_save.Location = new System.Drawing.Point(300, 372);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(75, 23);
            this.bt_save.TabIndex = 3;
            this.bt_save.Text = "保存为...";
            this.bt_save.UseVisualStyleBackColor = true;
            this.bt_save.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // pb_tts
            // 
            this.pb_tts.Location = new System.Drawing.Point(12, 351);
            this.pb_tts.Name = "pb_tts";
            this.pb_tts.Size = new System.Drawing.Size(363, 10);
            this.pb_tts.TabIndex = 4;
            // 
            // bt_open
            // 
            this.bt_open.Location = new System.Drawing.Point(12, 372);
            this.bt_open.Name = "bt_open";
            this.bt_open.Size = new System.Drawing.Size(75, 23);
            this.bt_open.TabIndex = 5;
            this.bt_open.Text = "打开";
            this.bt_open.UseVisualStyleBackColor = true;
            this.bt_open.Click += new System.EventHandler(this.bt_open_Click);
            // 
            // FormTTS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 402);
            this.Controls.Add(this.bt_open);
            this.Controls.Add(this.pb_tts);
            this.Controls.Add(this.bt_save);
            this.Controls.Add(this.bt_stop);
            this.Controls.Add(this.tb_content);
            this.Controls.Add(this.bt_play_pause);
            this.Name = "FormTTS";
            this.Text = "语音合成";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_play_pause;
        private System.Windows.Forms.TextBox tb_content;
        private System.Windows.Forms.Button bt_stop;
        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.ProgressBar pb_tts;
        private System.Windows.Forms.Button bt_open;
    }
}

