using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;
using System.IO.Ports;

namespace facebook_displayer
{


    public partial class Facebook_displayer : Form
    {
        public string caly_tekst="";
        public string tekst;
        public Facebook_displayer()
        {
            serialPort1 = new SerialPort();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getAvailablePorts();
        }

        public void getAvailablePorts()
        {
            comboBox1.Items.Clear();
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string OAuthURL = @"https://www.facebook.com/v2.3/dialog/oauth?client_id=1373520086004317&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token&scope=email";
            WebFacebook.Navigate(OAuthURL);
        }
        string access_token;

        private void WebFacebook_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(WebFacebook.Url.AbsoluteUri.Contains("access_token"))
            {
         
                string url1 = WebFacebook.Url.AbsoluteUri;
                string url2 = url1.Substring(url1.IndexOf("access_token") + 13);
                access_token = url2.Substring(0, url2.IndexOf("&"));
                //MessageBox.Show("access_token = " + access_token);

                FacebookClient fb = new FacebookClient(access_token);
                dynamic data = fb.Get("/me");
                MessageBox.Show("Zalogowano jako " + data.name);
                btn_Get_List.Enabled = true;
                btnSignIn.Enabled = false;
                btnClear.Enabled = true;
            }
        }
     
        private void btn_Get_List_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Start();
                /*--------POBIERANIE LISTY POWIADOMIEN (NIEAUTOMATYCZNE)--------*/
                //FacebookClient fb = new FacebookClient(access_token);
                //GET TOKEN: https://developers.facebook.com/tools/explorer/?method=GET&path=me%2Fstatuses&version=v2.9
                FacebookClient fb = new FacebookClient("EAACEdEose0cBAP3jTTgqgO35eaq2iIZAtySuJPQnCMTfu0FGBvBkJZCVKYnz3w01ZAEqZBZANOJaZBPBB1pZCYOF72MUdALd4qzJTNrf8tUg7TTJbZAH9qO2JfDxv2W8YzBKuNWtpJYaIyb5yJc3EPqbwssEhByO0Fu0ASPR793Oe1llktlEpkuCkiol4ObaJPQZD");
                dynamic notifications = fb.Get("/v2.3/me/notifications");
                int notification_count = (int)notifications.data.Count;
                caly_tekst = "";
                lstNotificationList.Items.Clear();
                btn_wyslij_liste.Enabled = true;
                for (int i = 0; i < notification_count; i++)
                {
                    caly_tekst += "*" + i + "*" + notifications.data[i].title;
                    string notification_name = notifications.data[i].title;
                    lstNotificationList.Items.Add(notification_name);
                }
                txtBox1.Text = caly_tekst;
                /*--------------------------------------------------------------*/
            }
            catch(UnauthorizedAccessException)
            {
                txtBox1.Clear();
                txtBox1.Text = "UnauthorizedAccessException";
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void brnClear_Click(object sender, EventArgs e)
        {
            lstNotificationList.Items.Clear();
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btn_Get_List_Click(sender, e);

            if(serialPort1.IsOpen)
            {
                if (button5.Text == "On" && btn_wyslij_liste.Enabled == true)
                {
                    button5_Click(sender, e);
                }
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == true)
                {
                    if (textBox1.Text != "")
                    {
                        serialPort1.WriteLine(textBox1.Text);
                        tekst = textBox1.Text;//-DO ZMIANY-//
                        textBox1.Clear();
                    }
                }
                else
                {
                    textBox1.Text = "Port jest zamkniety!";
                }
            }
            catch (TimeoutException)
            {
                textBox2.Text = "Timeout Exception";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == true)
                {
                    textBox2.Text = serialPort1.ReadExisting();
                    textBox2.Text = tekst; //-DO ZMIANY-//
                }
                else
                {
                    textBox2.Text = "Port jest zamkniety!";
                }
            }
            catch (TimeoutException)
            {
                textBox2.Text = "Timeout Exception";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    textBox2.Text = "Wybierz port";
                }
                else
                {
                    textBox1.Text = "";
                    if(caly_tekst!="")
                    {
                        btn_wyslij_liste.Enabled = true;
                    }
                    else if(caly_tekst == "")
                    {
                        btn_wyslij_liste.Enabled = false;
                    }
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    progressBar1.Value = 100;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    textBox1.Enabled = true;
                    button3.Enabled = false;
                    button4.Enabled = true;
                    textBox2.Clear();
                }
            }
            catch (UnauthorizedAccessException)
            {
                textBox2.Text = "Unauthorized Access Exception";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tekst = "";
            serialPort1.Close();
            progressBar1.Value = 0;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            button3.Enabled = true;
            textBox1.Enabled = false;
            btn_wyslij_liste.Enabled = false;
            textBox2.Clear();
            textBox1.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == true)
                { 
                    btn_wyslij_liste.Enabled = true;
                    serialPort1.WriteLine(caly_tekst);
                    tekst = caly_tekst;//-DO ZMIANY-//
                    //textBox1.Clear();
                }
                else
                {
                    textBox1.Text = "Port jest zamkniety!";
                }
            }
            catch (TimeoutException)
            {
                textBox2.Text = "Timeout Exception";
            }
        }

        private void Facebook_displayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }



        private void btn_scan_Click(object sender, EventArgs e)
        {
            getAvailablePorts();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if(button5.Text=="On")
            {
                button5.Text = "Off";
            }
            else if(button5.Text=="Off")
            {
                button5.Text = "On";
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
