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

        public static string Aname = "";
        public static string Bname = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;
            Random rand = new Random();

            for (int i = 0; i <54; i++)
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
                if(i<27)
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
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                        + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                        + "Integrated Security=True";
                        cn.Open();
                        SqlDataAdapter da = new SqlDataAdapter("select * from playerB", cn);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "playerB");
                        string[] point = new string[ds.Tables["playerB"].Rows.Count];
                        for (int i = 0; i < point.Length; i++)
                        {
                            point[i] = ds.Tables["playerB"].Rows[i]["points"].ToString().Trim(); //SQL字串轉成C#字串會有空格 必須刪掉
                        }

                        var groupbyBResult = from s in point.GroupBy(c => c) select s; //LINQ

                        foreach (var s in groupbyBResult)
                        {
                            if (s.Key != "g" || s.Key != "gg")
                            {
                                if (s.Count() % 2 == 0 && s.Count() > 1)
                                {
                                    string cmdstr = $"delete From playerB WHERE points ='{s.Key}'"; //s.key是陣列的某個值 因為GroupBy
                                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                                    cmd.ExecuteNonQuery();
                                }
                                else if (s.Count() % 2 != 0 && s.Count() > 1)
                                {
                                    string cmdstr = $"DELETE playerB where id NOT IN (Select Max(id) From [playerB] Group By points)";
                                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        Player1 p1 = new Player1();
                        p1.Visible = true;

                    }
                }
                else if (radioButton2.Checked == true)
                {
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                        + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                        + "Integrated Security=True";
                        cn.Open();
                        SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "playerA");
                        string[] point = new string[ds.Tables["playerA"].Rows.Count];
                        //建立存放points的陣列
                        for (int i = 0; i < point.Length; i++)
                        {
                            point[i] = ds.Tables["playerA"].Rows[i]["points"].ToString().Trim();
                            //把資料表points的資料一筆一筆存入
                        }

                        //LINQ
                        var groupbyAResult = from s in point.GroupBy(c => c) select s;
                        foreach (var s in groupbyAResult) //統計points重複出現的次數
                        {
                            if (s.Key != "g" || s.Key != "gg") //如果point不等於鬼牌才做刪除動作(鬼牌是gg.gif)
                            {
                                if (s.Count() % 2 == 0 && s.Count() > 1)
                                //如果這個點數出現次數是偶數而且大於1張
                                {
                                    string cmdstr = $"delete From playerA WHERE points ='{s.Key}'";
                                    //把這個點數的牌都刪除
                                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                                    cmd.ExecuteNonQuery();
                                }
                                else if (s.Count() % 2 != 0 && s.Count() > 1)
                                //如果這個點數出現次數是奇數而且大於2張
                                {
                                    string cmdstr = $"DELETE playerA where id NOT IN (Select Max(id) From [playerA] Group By points)";
                                    //刪除這個點數的牌但是有留一張

                                    SqlCommand cmd = new SqlCommand(cmdstr, cn);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    Player2 p2 = new Player2();
                    p2.Visible = true;
                }
            }
        }
        public string playerAname = Aname;
        public string playerBname = Bname;
    }
}
