using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices; //либа, управляющая движением формы
using System.Net.Mail; //либа, управляющая Zender
using Zender.Mail; //либа, управляющая Zender
using System.IO; //либа, управляющая последней сессией
using System.Text.RegularExpressions; //либа, управляющая парсингом
using System.Security.Cryptography; //либа, для шифровки/дешифровки последней сессии
using System.Net; //либа, для отправки POST-запросов на сервер отправки СМС

namespace CorpMail
{
    public partial class Form3 : Form
    {
        public static List<string> emails = new List<string>(); //init лист для почт
        public static List<string> tels = new List<string>(); //init лист для телефонов
        public int i = 0; //init счетчик кол-ва почт в прочитанном файле
        public int t = 0; //init счетчик кол-ва телефонов в прочитанном файле
        public int d = 0; //init счетчик id почты для отправки
        public int s = 0; //init счетчик кол-ва почты для вывода в textbox
        public const int WM_NCLBUTTONDOWN = 0xA1; //двигаем форму
        public const int HT_CAPTION = 0x2; //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam); //двигаем форму
        [DllImportAttribute("user32.dll")] //двигаем форму
        public static extern bool ReleaseCapture(); //двигаем форму
        public static string password = Form2.passwords[Form2.index]; //читаем пароль, который был использован при входе в систему
        public Form3()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e) //закрываем приложение
        {
            string curDir = Directory.GetCurrentDirectory();
            string writepath = String.Format(@"{0}\last_email_" + Form2.index + ".session", curDir); //init путь для сохранения сессии
            string writepath2 = String.Format(@"{0}\last_tel_" + Form2.index + ".session", curDir); //init путь для сохранения сессии
            textBox2.Text = null; //сброс поля для записи почт
            s = 0; //сброс счетчика от предыдущих значений
            while (s != i) //пока кол-во спарсенных значений не равно счетчику почт занесенных в текстбокс
            {
                if (s != i - 1) //fix последней пустой строки
                {
                    textBox2.Text = textBox2.Text + emails[s] + "\r\n"; //заносим в текстбокс для просмотра
                }
                else
                {
                    textBox2.Text = textBox2.Text + emails[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                }
                s++; //счетчик значений текстбокса
            }
            using (StreamWriter sw = new StreamWriter(writepath, false, System.Text.Encoding.Default))
            {
                sw.Write(Shifrovka(textBox2.Text, password)); //пишем SHA-1 последнюю сессию в last_email%pass_number%.session, паролем на шифровку служит пароль, использованный при входе в систему
            }
            textBox2.Text = null; //сброс поля для записи телефонов
            s = 0; //сброс счетчика для значений телефона
            while (s != t) //пока кол-во спарсенных значений не равно счетчику телефонов занесенных в текстбокс
            {
                if (s != t - 1) //fix последней пустой строки
                {
                    textBox2.Text = textBox2.Text + tels[s] + "\r\n"; //заносим в текстбокс для просмотра
                }
                else
                {
                    textBox2.Text = textBox2.Text + tels[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                }
                s++; //счетчик значений текстбокса
            }
            using (StreamWriter sw = new StreamWriter(writepath2, false, System.Text.Encoding.Default))
            {
                sw.Write(Shifrovka(textBox2.Text, password)); //пишем SHA-1 последнюю сессию в last_tel%pass_number%.session, паролем на шифровку служит пароль, использованный при входе в систему
            }
            Application.Exit();
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

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4(); //tinyMCE
            frm.Show();
            timer1.Enabled = true; //таймер для обновления кода из tinyMCE
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Form4.html; //обновляем html из tinyMCE
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || textBox1.Text == "Текст сообщения..." || textBox3.Text == "Тема сообщения..." || textBox1.Text == "" || textBox3.Text == "") //фиксит битые значения комбобокса и неотредактированные тема и сообщение
            {
                MessageBox.Show("У вас есть пустые или неотредактированные поля, необходимо заполнить их!");
            }
            else if (comboBox1.SelectedItem.ToString() == "Последняя сессия" || comboBox1.SelectedItem.ToString() == "Импорт файла") //передаем значения последней сессии или live-парсинга зендеру
            {
                while (d != i) //пока счетчик id почты для отправки почты не равен общему кол-ву почт, делаем...
                {
                    ZenderMessage message = new ZenderMessage("17395e9a-f6c1-4271-9e27-4dd736152fd1"); //API-key от Zender, получить можно при регистрации на http://zender.sharptag.com/
                    MailAddress from = new MailAddress("dmkhnk@gmail.com"); //Почта отправителя и по совместительству email, использованный при регистрации Zender
                    MailAddress to = new MailAddress(emails[d]); //таргет почта
                    message.From = from; //zender
                    message.To.Add(to); //zender
                    message.Subject = textBox3.Text; //заголовок письма
                    message.Body = textBox1.Text + "<br><p>С уважением, " + Form2.endings[Form2.index] + ".</p>"; //тело письма + окончание письма, с идентификацией текущего пользователя
                    message.IsBodyHtml = true; //труъ-html
                    message.SendMailAsync(); //отправка в zender
                    d++; //счетчик +1 к id почты для отправки
                }
                d = 0; //сброс счетчика для расчета телефона
                while (d != t) //пока счетчик id телефона для отправки смс не равен общему кол-ву телефонов, делаем...
                {
                    POST("https://gate.smsaero.ru/send/", "user=dmkhnk@gmail.com&password=CC1CEqAPEzuDcAUZ7Ah39yeYJOSB&to=7" + tels[d] + "&text=" + textBox1.Text + "&from=news");
                    d++; //счетчик +1 к id телефона для отправки
                }
                MessageBox.Show ("Успешно выполнено!");
            }
            else //костыль для предотвращения введения своих значений в комбобокс
            {
                MessageBox.Show("Проверьте правильность введенных данных!");
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Последняя сессия") //последняя сессия
            {
                string curDir = Directory.GetCurrentDirectory();
                string path = String.Format(@"{0}\last_email_" + Form2.index + ".session", curDir); //init путь к сохраненной сессии
                string path2 = String.Format(@"{0}\last_tel_" + Form2.index + ".session", curDir); //init путь к сохраненной сессии
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default)) //читаем последнюю сессию
                {
                    string line; //init линия в прочитанном файле
                    while ((line = sr.ReadLine()) != null) //пока читается, делаем...
                    {
                        try //ох уж этот C# | Try-catch method
                        {
                            emails.Add(DeShifrovka(line, password)); //добавление почты в глобал лист, дешифруя SHA-1, если это была последняя сессия сего юзера
                        }
                        catch (CryptographicException) //ловим ошибку дешифровки при подмене последней сессии и выполняем...
                        {
                            MessageBox.Show("Последняя сессия принадлежит другому пользователю! Выполните повторный импорт файла.");
                            i--; //фикс счетчика, магия, не иначе
                        }
                        catch (FormatException) //ловим ошибку дешифровки при повреждении последней сессии и выполняем...
                        {
                            MessageBox.Show("Последняя сессия повреждена! Выполните повторный импорт файла.");
                            i--; //фикс счетчика, магия, не иначе
                        }
                        i++; //счетчик кол-ва почт в прочитанном файле
                    }
                }
                using (StreamReader sr = new StreamReader(path2, System.Text.Encoding.Default)) //читаем последнюю сессию
                {
                    string line; //init линия в прочитанном файле
                    while ((line = sr.ReadLine()) != null) //пока читается, делаем...
                    {
                        try //ох уж этот C# | Try-catch method
                        {
                            tels.Add(DeShifrovka(line, password)); //добавление почты в глобал лист, дешифруя SHA-1, если это была последняя сессия сего юзера
                        }
                        catch (CryptographicException) //ловим ошибку дешифровки при подмене последней сессии и выполняем...
                        {
                            MessageBox.Show("Последняя сессия принадлежит другому пользователю! Выполните повторный импорт файла.");
                            t--; //фикс счетчика, магия, не иначе
                        }
                        catch (FormatException) //ловим ошибку дешифровки при повреждении последней сессии и выполняем...
                        {
                            MessageBox.Show("Последняя сессия повреждена! Выполните повторный импорт файла.");
                            t--; //фикс счетчика, магия, не иначе
                        }
                        t++; //счетчик кол-ва почт в прочитанном файле
                    }
                }
                while (s != i)
                {
                    if (s != i - 1 || t != 0) //fix последней пустой строки
                    {
                        textBox2.Text = textBox2.Text + emails[s] + "\r\n"; //заносим в текстбокс для просмотра
                    }
                    else
                    {
                        textBox2.Text = textBox2.Text + emails[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                    }
                    s++; //счетчик значений текстбокса
                }
                s = 0; //сброс счетчика для телефонов
                while (s != t)
                {
                    if (s != t - 1) //fix последней пустой строки
                    {
                        textBox2.Text = textBox2.Text + tels[s] + "\r\n"; //заносим в текстбокс для просмотра
                    }
                    else
                    {
                        textBox2.Text = textBox2.Text + tels[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                    }
                    s++; //счетчик значений текстбокса
                }
            }
            else if (comboBox1.SelectedItem.ToString() == "Импорт файла") //live-parsing
            {
                string file; //путь файла
                OpenFileDialog openFileDialog = new OpenFileDialog(); //init окно выбора файла
                if (openFileDialog.ShowDialog() == DialogResult.OK) //выбираем файл
                {
                    file = File.ReadAllText(openFileDialog.FileName); //читаем весь текст с файла
                    Regex regex = new Regex(@"\w+[a-zA-Z0-9-_.]+@+\w+[a-zA-Z0-9].[a-zA-Z]{2,4}"); //парсер почты с любого файла, маска почта@домен.зона
                    Match match = regex.Match(file); //парсим
                    while (match.Success) //пока парсится, делаем...
                    {
                        emails.Add(match.Value); //заносим спарсенное в глобал лист
                        match = match.NextMatch(); //переход к следующему спарсенному значению
                        i++; //счетчик кол-ва почт в прочитанном файле
                    }

                    Regex regex2 = new Regex(@"9+[0-9]{9}"); //парсер телефонов с любого файла, маска 9xxxxxxxxx
                    Match match2 = regex2.Match(file); //парсим
                    while (match2.Success) //пока парсится, делаем...
                    {
                        tels.Add(match2.Value); //заносим спарсенное в глобал лист
                        match2 = match2.NextMatch(); //переход к следующему спарсенному значению
                        t++; //счетчик кол-ва телефонов в прочитанном файле
                    }

                }
                while (s != i) //пока кол-во спарсенных значений не равно счетчику почт занесенных в текстбокс
                {
                    if (s != i-1 || t != 0) //fix последней пустой строки
                    {
                        textBox2.Text = textBox2.Text + emails[s] + "\r\n"; //заносим в текстбокс для просмотра
                    }
                    else
                    {
                        textBox2.Text = textBox2.Text + emails[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                    }
                    s++; //счетчик значений текстбокса
                }
                s = 0; //сброс счетчика для значений телефона
                while (s != t) //пока кол-во спарсенных значений не равно счетчику телефонов занесенных в текстбокс
                {
                    if (s != t - 1) //fix последней пустой строки
                    {
                        textBox2.Text = textBox2.Text + tels[s] + "\r\n"; //заносим в текстбокс для просмотра
                    }
                    else
                    {
                        textBox2.Text = textBox2.Text + tels[s]; //заносим в текстбокс последнее значение, без специальных ключей сноса строки
                    }
                    s++; //счетчик значений текстбокса
                }
            }
            if (textBox2.Text == null || textBox2.Text == "" || textBox2.Text == "\r\n") //если не было найдено ни одной почти в сессии или после парсинга
            {
                MessageBox.Show("Не было найдено ни одного получателя!");
            }
            else
            {
                comboBox1.Enabled = false; //запрещаем изменение, костыль для сохранения сессии, магия, не иначе
                textBox2.Visible = true; //делаем видимым textbox для просмотра почт и..
                label1.Visible = true; //..надпись над текстбоксом
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5(); //about
            frm.Show();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------
        //МЕТОД ШИФРОВАНИЯ СТРОКИ START
        public static string Shifrovka(string ishText, string pass,
               string cryptographicAlgorithm = "SHA1",
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#", //вектор для шифровки должен быть уникальным, поэтому рекомендуется его изменить (не обязательно) | должен совпадать с вектором метода дешифровки, находящийся ниже...
               int keySize = 256) //размер ключа | должен совпадать размером ключа метода шифровки, находящийся выше...
        {
            string sol = password + "VRba7hXCaa4vIvlXFr"; //соль для шифровки должна быть уникальна, поэтому крайне рекомендуется его изменить | должен совпадать с солью метода дешифровки, находящийся ниже...
            if (string.IsNullOrEmpty(ishText))
                return ""; //фикс реверс-инжиниринга соли и вектора путем подбора

            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] ishTextB = Encoding.UTF8.GetBytes(ishText);

            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);
            RijndaelManaged symmK = new RijndaelManaged();
            symmK.Mode = CipherMode.CBC;

            byte[] cipherTextBytes = null;

            using (ICryptoTransform encryptor = symmK.CreateEncryptor(keyBytes, initVecB))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(ishTextB, 0, ishTextB.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmK.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }
        //МЕТОД ШИФРОВАНИЯ СТРОКИ END
        //-------------------------------------------------------------------------------------------------------------------------------------------------
        //МЕТОД ДЕШИФРОВАНИЯ СТРОКИ START
        public static string DeShifrovka(string ciphText, string pass,
               string cryptographicAlgorithm = "SHA1", //криптографический алгоритм | должен совпадать алгоритмом метода шифровки, находящийся выше...
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#", //вектор для дешифровки должен быть уникальным, поэтому рекомендуется его изменить (не обязательно) | должен совпадать с вектором метода шифровки, находящийся выше...
               int keySize = 256) //размер ключа | должен совпадать размером ключа метода шифровки, находящийся выше...
        {
            string sol = password + "VRba7hXCaa4vIvlXFr"; //соль для дешифровки должна быть уникальна, поэтому крайне рекомендуется его изменить | должен совпадать с солью метода шифровки, находящийся выше...
            if (string.IsNullOrEmpty(ciphText))
                return ""; //фикс реверс-инжиниринга соли и вектора путем подбора

            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] cipherTextBytes = Convert.FromBase64String(ciphText);

            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);

            RijndaelManaged symmK = new RijndaelManaged();
            symmK.Mode = CipherMode.CBC;

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;

            using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
            {
                using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                    {
                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        mSt.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmK.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }
        //МЕТОД ДЕШИФРОВАНИЯ СТРОКИ END
        //-------------------------------------------------------------------------------------------------------------------------------------------------

        private void Form3_Load(object sender, EventArgs e) //жуткий костыль, ибо было лень гуглить
        {
            string curDir = Directory.GetCurrentDirectory();
            string checkpath = String.Format(@"{0}\last_email_" + Form2.index + ".session", curDir); //init путь для сохранененной сессии
            if (!File.Exists(checkpath)) //если последняя сессия отсутствует, то..
            {
                comboBox1.Items.Clear(); //убираем все в комбобоксе и..
                comboBox1.Items.Add("Импорт файла"); //..оставляем только импорт
            }
        }

        private static string POST(string Url, string Data)
        {
            WebRequest req = System.Net.WebRequest.Create(Url);
            req.Method = "POST";
            req.Timeout = 100000;
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = UTF8Encoding.UTF8.GetBytes(Data);
            req.ContentLength = sentData.Length;
            Stream sendStream = req.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            WebResponse res = req.GetResponse();
            Stream ReceiveStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);
            //Кодировка указывается в зависимости от кодировки ответа сервера
            Char[] read = new Char[256];
            int count = sr.Read(read, 0, 256);
            string Out = String.Empty;
            while (count > 0)
            {
                String str = new String(read, 0, count);
                Out += str;
                count = sr.Read(read, 0, 256);
            }
            return Out;
        }

    }
}
