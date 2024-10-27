using FinalProject.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinalProject.service;

namespace FinalProject
{
    public partial class Form1 : Form
    {

        public Form1()
        {

            InitializeComponent();
        }
        int[] rand_poker = new int[54];
        int[] playerA_poker = new int[27];
        int[] playerB_poker = new int[27];
        Common common_service = new Common();

        public static string Aname = "";
        public static string Bname = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;
            Random rand = new Random();

            for (int i = 0; i < 54; i++)
            {
                rand_poker[i] = rand.Next(1, 55);

                for (int j = 0; j < i; j++)
                {
                    while (rand_poker[j] == rand_poker[i])
                    {
                        j = 0;
                        rand_poker[i] = rand.Next(1, 55);
                    }
                }
            }
            int count = 0;
            for (int i = 0; i < 54; i++)
            {
                if (i < 27)
                {
                    playerA_poker[i] = rand_poker[i];
                }
                else
                {
                    playerB_poker[count] = rand_poker[i];
                    count += 1;
                }
            }
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                + "Integrated Security=True";
                cn.Open();
                SqlCommand deleteAcmd = new SqlCommand($"delete from playerA", cn);
                deleteAcmd.ExecuteNonQuery();
                SqlCommand deleteBcmd = new SqlCommand($"delete from playerB", cn);
                deleteBcmd.ExecuteNonQuery();
                for (int i = 0; i < 27; i++)
                {
                    string cmdstr = $"insert into playerA(points,suits,joker) select points,suits,joker from pokers where id='{playerA_poker[i]}'";
                    //依序把亂數後的牌放入playerA
                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                    cmd.ExecuteNonQuery();
                }
                for (int i = 0; i < 27; i++)
                {
                    string cmdstr = $"insert into playerB(points,suits,joker) select points,suits,joker from pokers where id='{playerB_poker[i]}'";
                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("請輸入名字!");
            }
            else
            {
                Aname = textBox1.Text;
                Bname = textBox2.Text;
                this.Visible = false;
                if (radioButton1.Checked == true)
                {
                    common_service.RemoveDuplicateCards("playerB");
                    Player1 p1 = new Player1();
                    p1.Visible = true;
                }
                else if (radioButton2.Checked == true)
                {
                    common_service.RemoveDuplicateCards("playerA");
                    Player2 p2 = new Player2();
                    p2.Visible = true;
                }
            }
        }
        public string playerAname = Aname;
        public string playerBname = Bname;
    }
}
