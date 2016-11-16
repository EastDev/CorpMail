using System;
using System.Windows.Forms;

namespace CorpMail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e) //таймер-костыль
        {
            this.Hide(); //убираем форму
            Form2 frm = new Form2(); //форма логина
            frm.Show();
            timer1.Enabled = false; //выключаем таймер-костыль
        }
    }
}
