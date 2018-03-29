using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;



/*
 Сообщение: 
 Программа находит диалог с пользователем из Skype,
 ну удалось создать запрос в бд,для того чтобы разделить
 сообщения на входящие и исходящие,из-за этого я не стал 
 создавать локальную базу данных. 

 Skype пишет все данные в main.db,(данная db в принципе устарела)
 выдернул все  на прямую,
 редактором посмотрел какие поля сущ и с кем взаимосвязанны

 правильно составить запрос для отсортировки выходящих и исходящих сообщений
 не получилось.
 
 Пока что не справился с задачей,еще не много подумаю,
 может быть получиться составить правильно запрос к бд.

В сторону SKYPE Api URL, APi и тд,смотрел не нашел,
того что мне нужно!


Или может есть идеи,как это можно реализовать?


PS Программа не работает во встроенном Skype под win 10
запись в БД отсутствует,как я понимаю,все данные обрабатываются уже на самом сервере)

Извините, за срыв  дедлайна!
     
*/



namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;


        SQLiteConnection db = new SQLiteConnection("Data Source=main.db; Version=3");

       

                 SQLiteCommand command = new SQLiteCommand();
        



        private SQLiteConnection DB;
        public Form1()
        {
            InitializeComponent();
            Searchdb();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (Process.GetProcessesByName("Skype").Count() != 0)
            {
                //Получить доступ к Skype
                axSkype1.Attach();
                
                label1.Text = "Подключено!";
                label2.Text = ("Приветствую вас : " + axSkype1.CurrentUserProfile.FullName);
            }

            else   MessageBox.Show("Skype не запущен!");
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Skype свернется в трей
            axSkype1.Client.Minimize();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            axSkype1.Client.Shutdown();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

         
        private void USER()
        {

            if (textBox1.Text != "")
            {
                DataBase();
            }

        }

        //Подключение к базе данных и обработка запроса
        public void DataBase()
        {
              
                SQLiteConnection db = new SQLiteConnection("Data Source=main.db; Version=3");
               
                 db.Open();

                 SQLiteCommand command = new SQLiteCommand();
                 SQLiteCommand cmd = db.CreateCommand();

                //Запрос к бд
                cmd.CommandText = string.Format("select author,from_dispname,body_xml from Messages where chatname like '%'|| @chatname || '%' order by timestamp ");
            cmd.Parameters.Add("@chatname", System.Data.DbType.String).Value += textBox1.Text.ToUpper();
            SQLiteDataReader SQL = cmd.ExecuteReader();
                if (SQL.HasRows)
                {
                    //Чтение данных 
                    while (SQL.Read())
                    {

                        richTextBox1.Text += "" + SQL["body_xml"] + "\r\n";

                        richTextBox2.Text += "" + SQL["from_dispname"] + "\r\n";
                    }

                }
                else  richTextBox1.Text = "Нет таких";
                
                  
                
            
            db.Close();
        }



        //Подключение к базе данных и обработка запроса и вывод данных в табл
        private void LoadDataGrid()
        {
            DB = new SQLiteConnection("Data Source=main.db; Version=3");
            DB.Open();

         

            if (textBox4.Text != "")
            {
                SQLiteCommand CMD = DB.CreateCommand();
                CMD.CommandText = string.Format("select id,author,chatname,body_xml from Messages where chatname like '%'|| @Name || '%' order by timestamp ");
                CMD.Parameters.Add("@Name", System.Data.DbType.String).Value = textBox4.Text.ToUpper();
                 CMD.ExecuteNonQuery();




                SQLiteDataReader SQL = CMD.ExecuteReader();
                List<string[]> data = new List<string[]>();

                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                      
                        data.Add(new string[4]);
                        data[data.Count - 1][0] = SQL["id"].ToString();
                        data[data.Count - 1][1] = SQL["author"].ToString();
                        data[data.Count - 1][2] = SQL["chatname"].ToString();
                        data[data.Count - 1][3] = SQL["body_xml"].ToString();



                    }

                    SQL.Close();

                    DB.Close();

                    foreach (string[] s in data)
                        dataGridView2.Rows.Add(s);

                }
                else
                {
                    richTextBox1.Text = "Нет таких";
                }

            }

            

        }


        //Search users
        private void button5_Click(object sender, EventArgs e)
        {
            USER();
           
       }


        //Поиск db на компьютере пользователя
        //и копирование db к программе
        public void Searchdb()
        {
            try
            {
                //Поиск базы данных на компьютере пользователя,
               // Если база сущестсует,копируем в папку с программной
                string user_Windows = SystemInformation.UserName;
                string fileMain = @"main.db";
                string[] files = Directory.GetFiles(@"C:\Users\" + user_Windows + @"\AppData\Roaming\Skype\", fileMain, SearchOption.AllDirectories);
               
                foreach (string fil in files) File.Copy(fil, fileMain);
                
            }
            catch (System.IO.IOException ex){  }

          
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
        }

        
        private void button4_Click(object sender, EventArgs e)
        {
            //Запуск
            axSkype1.Client.Start(true,true);
        }

        // обработка запроса 
        private void button8_Click(object sender, EventArgs e)
        {
            LoadDataGrid();
        }

        //Вывод всех сообщений из Skype
        private void button7_Click(object sender, EventArgs e)
        {
            DataBase();
        }

     
        private void button10_Click(object sender, EventArgs e)
        {

            TextWriter writer = new StreamWriter(@"Save.txt");
            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView2.Columns.Count; j++)
                {
                    writer.Write("\t" + dataGridView2.Rows[i].Cells[j].Value.ToString() + "\t" + "|");
                }
                writer.WriteLine("");
                writer.WriteLine("-----------------------------------------------------");
            }
            writer.Close();
            MessageBox.Show("Save file");
            
        }
    }
}
