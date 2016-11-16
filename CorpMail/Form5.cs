using System;
using System.Windows.Forms;
using System.Runtime.InteropServices; //либа, управляющая движением формы

namespace CorpMail
{
    public partial class Form5 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1; //двигаем форму
        public const int HT_CAPTION = 0x2; //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern bool ReleaseCapture(); //двигаем форму
        public Form5()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e) //кнопка закрытия формы
        {
            this.Close(); //закрываем текущую форму
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

        private void pictureBox3_Click(object sender, EventArgs e) //кнопка минимизации окна
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e) //костыли для минимизации окна + наведение на них
        {
            pictureBox3.Image = CorpMail.Properties.Resources.minusup;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e) //костыли для закрытия окна - наведение на них
        {
            pictureBox3.Image = CorpMail.Properties.Resources.minus;
        }

        private void pictureBox2_Click(object sender, EventArgs e) //пасхалка, когда нечего делать
        {
            MessageBox.Show("За время разработки данного приложения, оно было скомпилированно и собранно 414 раз, было выпито 43.5 кружки кофе, съедено 121 печенье и проведено с пользой аж 13 ночей!", "Пасхалка, однако!");
        }
    }
}
