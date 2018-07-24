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
            this.textBox_addresss = new System.Windows.Forms.TextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button_chat = new System.Windows.Forms.Button();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox_send = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button_clear = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.comboBox_rmi_name = new System.Windows.Forms.ComboBox();
            this.textBox_rmi_id = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel13.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_addresss
            // 
            this.textBox_addresss.Location = new System.Drawing.Point(88, 8);
            this.textBox_addresss.Name = "textBox_addresss";
            this.textBox_addresss.Size = new System.Drawing.Size(193, 21);
            this.textBox_addresss.TabIndex = 0;
            this.textBox_addresss.Text = "127.0.0.1";
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(368, 6);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(85, 23);
            this.button_connect.TabIndex = 2;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(493, 232);
            this.textBox2.TabIndex = 8;
            // 
            // button_chat
            // 
            this.button_chat.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_chat.Location = new System.Drawing.Point(407, 0);
            this.button_chat.Name = "button_chat";
            this.button_chat.Size = new System.Drawing.Size(86, 25);
            this.button_chat.TabIndex = 15;
            this.button_chat.Text = "Send";
            this.button_chat.UseVisualStyleBackColor = true;
            this.button_chat.Click += new System.EventHandler(this.button_chat_Click);
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(287, 8);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(75, 21);
            this.textBox_port.TabIndex = 1;
            this.textBox_port.Text = "5000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(4, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Send MSG";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(459, 6);
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
            // textBox_send
            // 
            this.textBox_send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_send.Location = new System.Drawing.Point(0, 0);
            this.textBox_send.Multiline = true;
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.Size = new System.Drawing.Size(493, 123);
            this.textBox_send.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox_addresss);
            this.panel1.Controls.Add(this.button_connect);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.textBox_port);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 35);
            this.panel1.TabIndex = 16;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel13);
            this.panel2.Controls.Add(this.panel8);
            this.panel2.Controls.Add(this.panel7);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 183);
            this.panel2.TabIndex = 17;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.comboBox_rmi_name);
            this.panel8.Controls.Add(this.textBox_rmi_id);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(88, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(493, 35);
            this.panel8.TabIndex = 21;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.button_chat);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(88, 158);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(493, 25);
            this.panel7.TabIndex = 20;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(581, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(10, 183);
            this.panel5.TabIndex = 19;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(88, 183);
            this.panel6.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(581, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(10, 272);
            this.panel3.TabIndex = 18;
            // 
            // button_clear
            // 
            this.button_clear.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_clear.Location = new System.Drawing.Point(495, 0);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(86, 30);
            this.button_clear.TabIndex = 0;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel12);
            this.panel4.Controls.Add(this.panel11);
            this.panel4.Controls.Add(this.panel10);
            this.panel4.Controls.Add(this.panel9);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 218);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(591, 272);
            this.panel4.TabIndex = 9;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.textBox2);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(88, 10);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(493, 232);
            this.panel12.TabIndex = 22;
            // 
            // panel11
            // 
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(88, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(493, 10);
            this.panel11.TabIndex = 21;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.label3);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(88, 242);
            this.panel10.TabIndex = 20;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.button_clear);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel9.Location = new System.Drawing.Point(0, 242);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(581, 30);
            this.panel9.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(4, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Debug MSG";
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.textBox_send);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(88, 35);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(493, 123);
            this.panel13.TabIndex = 22;
            // 
            // comboBox_rmi_name
            // 
            this.comboBox_rmi_name.FormattingEnabled = true;
            this.comboBox_rmi_name.Location = new System.Drawing.Point(0, 6);
            this.comboBox_rmi_name.Name = "comboBox_rmi_name";
            this.comboBox_rmi_name.Size = new System.Drawing.Size(274, 20);
            this.comboBox_rmi_name.TabIndex = 24;
            this.comboBox_rmi_name.SelectedIndexChanged += new System.EventHandler(this.comboBox_rmi_name_SelectedIndexChanged);
            // 
            // textBox_rmi_id
            // 
            this.textBox_rmi_id.BackColor = System.Drawing.Color.LightGray;
            this.textBox_rmi_id.Location = new System.Drawing.Point(280, 6);
            this.textBox_rmi_id.Name = "textBox_rmi_id";
            this.textBox_rmi_id.ReadOnly = true;
            this.textBox_rmi_id.Size = new System.Drawing.Size(85, 21);
            this.textBox_rmi_id.TabIndex = 23;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.ClientSize = new System.Drawing.Size(591, 490);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Test Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_addresss;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button_chat;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.ComboBox comboBox_rmi_name;
        private System.Windows.Forms.TextBox textBox_rmi_id;
    }
}

