using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices; //либа, управляющая движением формы
using System.IO;

namespace CorpMail
{
    public partial class Form4 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1; //двигаем форму
        public const int HT_CAPTION = 0x2; //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern bool ReleaseCapture(); //двигаем форму
        public static string html; //init глобальной переменной для вывода html кода из tinyMCE
        public Form4()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close(); //закрытие данной формы
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e) //двигаем форму
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e) //костыли для закрытия окна + наведение на них
        {
            pictureBox1.Image = CorpMail.Properties.Resources.closeup;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e) //костыли для закрытия окна - наведение на них
        {
            pictureBox1.Image = CorpMail.Properties.Resources.close;
        }

        private void pictureBox3_Click(object sender, EventArgs e) //костыли для минимизированного состояния окна
        {
            if (WindowState == FormWindowState.Maximized)
            {
                pictureBox4.Image = CorpMail.Properties.Resources.min;
            }
            else
            {
                pictureBox4.Image = CorpMail.Properties.Resources.max;
            }
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e) //костыли для минимизированного состояния окна + наведение на них
        {
            pictureBox3.Image = CorpMail.Properties.Resources.minusup;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e) //костыли для минимизированного состояния окна - наведение на них
        {
            pictureBox3.Image = CorpMail.Properties.Resources.minus;
        }

        private void pictureBox4_Click(object sender, EventArgs e) //костыли для максимизированного состояния окна
        {
            if (WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                pictureBox4.Image = CorpMail.Properties.Resources.max;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                pictureBox4.Image = CorpMail.Properties.Resources.min;
            }
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e) //костыли для максимизированного состояния окна + наведение на них
        {
            if (WindowState == FormWindowState.Maximized)
            {
                pictureBox4.Image = CorpMail.Properties.Resources.minup;
            }
            else
            {
                pictureBox4.Image = CorpMail.Properties.Resources.maxup;
            }
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e) //костыли для максимизированного состояния окна - наведение на них
        {
            if (WindowState == FormWindowState.Maximized)
            {
                pictureBox4.Image = CorpMail.Properties.Resources.min;
            }
            else
            {
                pictureBox4.Image = CorpMail.Properties.Resources.max;
            }
        }

        private void label2_MouseEnter(object sender, EventArgs e) //костыль цвета кнопки сохранить + белый
        {
            label2.ForeColor = Color.FromArgb(255, 255, 255);
        }

        private void label2_MouseLeave(object sender, EventArgs e) //костыль цвета кнопки сохранить - белый
        {
            label2.ForeColor = Color.FromArgb(196, 216, 233);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            dynamic data = webBrowser1.Document.InvokeScript("GetContent"); //отправка js запроса к tinyMCE на получение html кода из конструктора
            html = data; //назначаем выводу с js-запроса в глобальную переменную
            label2.Visible = false; //убираем кнопку сохранить
        }

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            label2.Visible = true; //если внесено хоть одно изменение в конструктор tinyMCE -> кнопка сохранить, появись!
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5(); //about
            frm.Show();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            string curDir = Directory.GetCurrentDirectory();
            webBrowser1.Url = new Uri(String.Format("file:///{0}/tinymce/tinymce.html", curDir)); //путь к tinyMCE
        }
    }
}
