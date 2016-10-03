namespace TestClient
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_addresss = new System.Windows.Forms.TextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.button_select = new System.Windows.Forms.Button();
            this.button_send = new System.Windows.Forms.Button();
            this.button_chat = new System.Windows.Forms.Button();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox_chat = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // textBox_addresss
            // 
            this.textBox_addresss.Location = new System.Drawing.Point(137, 10);
            this.textBox_addresss.Name = "textBox_addresss";
            this.textBox_addresss.Size = new System.Drawing.Size(140, 21);
            this.textBox_addresss.TabIndex = 0;
            this.textBox_addresss.Text = "127.0.0.1";
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(364, 8);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(75, 23);
            this.button_connect.TabIndex = 2;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 95);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(522, 254);
            this.textBox2.TabIndex = 8;
            // 
            // textBox_path
            // 
            this.textBox_path.Location = new System.Drawing.Point(137, 38);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.ReadOnly = true;
            this.textBox_path.Size = new System.Drawing.Size(235, 21);
            this.textBox_path.TabIndex = 5;
            // 
            // button_select
            // 
            this.button_select.Location = new System.Drawing.Point(378, 38);
            this.button_select.Name = "button_select";
            this.button_select.Size = new System.Drawing.Size(75, 23);
            this.button_select.TabIndex = 6;
            this.button_select.Text = "Load";
            this.button_select.UseVisualStyleBackColor = true;
            this.button_select.Click += new System.EventHandler(this.button_select_Click);
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(459, 37);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(75, 23);
            this.button_send.TabIndex = 7;
            this.button_send.Text = "Save";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_chat
            // 
            this.button_chat.Location = new System.Drawing.Point(459, 67);
            this.button_chat.Name = "button_chat";
            this.button_chat.Size = new System.Drawing.Size(75, 23);
            this.button_chat.TabIndex = 15;
            this.button_chat.Text = "Send";
            this.button_chat.UseVisualStyleBackColor = true;
            this.button_chat.Click += new System.EventHandler(this.button_chat_Click);
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(283, 10);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(75, 21);
            this.textBox_port.TabIndex = 1;
            this.textBox_port.Text = "5000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Server Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Chatting Message";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(12, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Send File To Server";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(445, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Disconnect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBox_chat
            // 
            this.textBox_chat.Location = new System.Drawing.Point(243, 69);
            this.textBox_chat.Name = "textBox_chat";
            this.textBox_chat.Size = new System.Drawing.Size(210, 21);
            this.textBox_chat.TabIndex = 14;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(137, 69);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(100, 21);
            this.textBox_name.TabIndex = 13;
            this.textBox_name.Text = "User";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.ClientSize = new System.Drawing.Size(546, 366);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.textBox_chat);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.button_chat);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_select);
            this.Controls.Add(this.textBox_path);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.textBox_addresss);
            this.Name = "Form1";
            this.Text = "Test Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_addresss;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.Button button_select;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_chat;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_chat;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

