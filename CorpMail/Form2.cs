using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices; //либа, управляющая движением формы
using System.Threading; //либа, управляющая величайшим костылем

namespace CorpMail
{
    public partial class Form2 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1; //двигаем форму
        public const int HT_CAPTION = 0x2; //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern bool ReleaseCapture(); //двигаем форму
        public static int index;
        //start задаем пароли программы
        public static String[] passwords = //можно добавлять неограниченное количество паролей, используя правила синтаксиса массивов в c#
        {
            "admin", //1 пользователь
            "testingpass", //2 пользователь
            "test" //3 пользователь
            //и так далее..
        };
        //end задаем пароли программы
        //start задаем подписи в конце письма для каждого пользователя
        public static String[] endings = //нужно добавлять подписи в идентичном порядке, который использовался выше, используя правила синтаксиса массивов в c#
        {
            "Директор АО \"Предприятие\", Иванов Иван Иванович", //1 пользователь
            "Зам. Директора АО \"Предприятие\", Карлов Карл Карлович", //2 пользователь
            "Тех. Администратор АО \"Предприятие\", Петров Петр Петрович" //3 пользователь
            //и так далее..
        };
        //end задаем подписи в конце письма для каждого пользователя
        public Form2()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e) //кнопка выхода из приложения
        {
            Application.Exit(); //выходим из приложения
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

        private void pictureBox3_MouseLeave(object sender, EventArgs e) //костыли для минимизации окна - наведение на них
        {
            pictureBox3.Image = CorpMail.Properties.Resources.minus;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            index = Array.IndexOf(passwords, textBox1.Text);
            if (index != -1) //если пароль правильный, делаем...
            {
                this.Close(); //закрываем эту форму
                Form3 frm = new Form3(); //основная форма
                frm.Show();
            }
            else if (textBox1.Text == "") //жуткий костыль, имитируем мигание поля пароля, если пустой пароль ---> магия, не иначе
            {
                pictureBox5.Visible = false; //убираем поле пароля
                Thread.Sleep(100); //ждем 100 мс
                pictureBox5.Visible = true; //поле пароля, появись!
            }
            else
            {
                label2.Visible = true; //+ Неверный пароль
                textBox1.Focus(); //фокус на поле пароля
                textBox1.SelectionStart = 0; //выбираем весь неверный пароль
                textBox1.SelectionLength = textBox1.Text.Length; //выбираем весь неверный пароль
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Visible = false; //если начато исправление пароля, убираем текст неверный пароль
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5(); //about
            frm.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Form6 frm = new Form6(); //справка
            frm.Show();
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.ForeColor = Color.FromArgb(255, 255, 255); //фикс цвета при наведении мыши на справку
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = Color.FromArgb(196, 216, 233); //фикс цвета при отстуствии наведения мыши на справку
        }
    }
}
