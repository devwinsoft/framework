using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devarc;

namespace EncryptViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_encrypt_Click(object sender, EventArgs e)
        {
            textBox_output.Text = Cryptor.Encrypt(textBox_input.Text);
        }

        private void button_decrypt_Click(object sender, EventArgs e)
        {
            textBox_output.Text = Cryptor.Decrypt(textBox_input.Text);
        }
    }
}
