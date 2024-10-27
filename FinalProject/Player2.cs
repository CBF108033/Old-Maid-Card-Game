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
    public partial class Player2 : Form
    {

        public Player2()
        {
            InitializeComponent();
        }
        string cnstr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DB.mdf;Integrated Security=True";
        void ShowData() //DataTable dataTable
        {
            listView1.Clear();
            imageList1.Images.Clear();
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                + "Integrated Security=True";
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from playerB", cn);
                DataSet ds = new DataSet();
                da.Fill(ds, "playerB");

                listView1.View = View.LargeIcon;
                imageList1.ImageSize = new System.Drawing.Size(60, 60);
                listView1.LargeImageList = imageList1;
                imageList1.ColorDepth = ColorDepth.Depth32Bit;

                for (int i = 0; i < ds.Tables["playerB"].Rows.Count; i++)
                {
                    string points = ds.Tables["playerB"].Rows[i]["points"].ToString();
                    points = points.Replace(" ", String.Empty);
                    string suits = ds.Tables["playerB"].Rows[i]["suits"].ToString();
                    suits = suits.Replace(" ", String.Empty);
                    if (points == "gg")
                    {
                        points = "g";
                    }
                    string test = "../../poker/" + points + suits + ".gif";
                    imageList1.Images.Add(Image.FromFile(test));
                    listView1.Items.Add(Path.GetFileName(test));
                    listView1.Items[i].ImageIndex = i;
                }
            }
        }
        void del_Poker()
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
                    point[i] = ds.Tables["playerB"].Rows[i]["points"].ToString().Trim();
                }
                foreach (var s in point.GroupBy(c => c))
                {
                    if (s.Key != "g" || s.Key!="gg")
                    {
                        if (s.Count() % 2 == 0 && s.Count() > 1)
                        {
                            string cmdstr = $"delete From playerB WHERE points ='{s.Key}'";
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
            }
        }
        int count = 0;
        public static int gameTime = 0;
        public int gametime2 = gameTime;

        private void Player2_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;

            Form1 form1 = new Form1();
            label1.Text = $"我的牌({form1.playerBname}) :";
            label3.Text = $"對方的牌({form1.playerAname}) :";

            Player1 player1 = new Player1();
            //判斷 抽出一對牌 要不要顯示
            //只有第一次才需要顯示 抽出一對牌
            if (player1.gametime2 > 0)  //不是第一個的話
            {
                button1.Visible = false;    //隱藏自動抽出成對牌
                button3.Visible = false;    //隱藏手動抽出成對牌
            }
            //else
            //{
            //    button2.Enabled = false;    //不能點 確認抽出 
            //    textBox1.Enabled = false;   //不能填 textBox
            //}
            //累計次數
            gameTime += 1;

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstr;
                cn.Open();
                string selectAstr = "select * from playerA";
                string selectBstr = "select * from playerB";
                DataSet ds = new DataSet();
                SqlDataAdapter daA = new SqlDataAdapter(selectAstr, cn);               
                daA.Fill(ds, "playerA");
                SqlDataAdapter daB = new SqlDataAdapter(selectBstr, cn);
                daB.Fill(ds, "playerB");

                ShowData();

                SqlCommand cmdA = new SqlCommand(selectAstr, cn);
                SqlDataReader dr = cmdA.ExecuteReader();

                while (dr.Read())
                {
                    count += 1;
                }
                dr.Close();
                cn.Close();
                pictureBox1.ImageLocation = @"../../pokerBack/" + count + ".png";
                //if (ds.Tables["playerB"].Rows.Count == 0)
                //{
                //    MessageBox.Show("玩家B贏了！");
                //    System.Environment.Exit(0);
                //}
                //if (ds.Tables["playerA"].Rows.Count == 0)
                //{
                //    MessageBox.Show("玩家A贏了！");
                //    System.Environment.Exit(0);
                //}               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //132.133把顯示牌的list清空,要不然下次再進入會一直顯示
            string id = "";
            string points = "";
            string suits = "";
            string joker = "";
            try
            {
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = cnstr;
                    string sqlstr = "select * from playerA";
                    SqlCommand cmd = new SqlCommand(sqlstr, cn);
                    cn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    int count2 = 1;
                    while (dr.Read())
                    {
                        if (count2 == Convert.ToInt32(textBox1.Text) && Convert.ToInt32(textBox1.Text) <= count && Convert.ToInt32(textBox1.Text) > 0)
                        {
                            id = dr["id"].ToString();
                            points = dr["points"].ToString();
                            suits = dr["suits"].ToString();
                            joker = dr["joker"].ToString();
                            break;
                        }
                        //else
                        //{
                        //    label1.Text += dr["points"];
                        //}
                        count2 += 1;
                    }
                }
                if (points != "" || suits != "" || joker != "")
                {
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = cnstr;
                        cn.Open();
                        //insert抽到的牌至B 可改預存程式
                        string insertStr = $"insert into playerB(points,suits,joker) values('{points}','{suits}','{joker}')";
                        SqlCommand cmd2 = new SqlCommand(insertStr, cn);
                        cmd2.ExecuteNonQuery();
                        //從A delete被抽到的牌 可改預存程式
                        string delectStr = $"delete from playerA where id='{id}'";
                        SqlCommand cmd3 = new SqlCommand(delectStr, cn);
                        cmd3.ExecuteNonQuery();
                    }
                    //ShowData();
                    this.Visible = false;
                    Exchange2 exchange2 = new Exchange2();
                    exchange2.Visible = true;
                    listView1.Clear();
                    imageList1.Images.Clear();
                }
                else
                {
                    MessageBox.Show("請輸入有效範圍的張數");
                }
            }
            catch
            {
                MessageBox.Show("請輸入有效張數");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //button2.Enabled = true;
            //textBox1.Enabled = true;
            del_Poker();
            ShowData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string firstPoint = "";
            string secondPoint = "";
            string thirdPoint = "";
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Selected == true && firstPoint == "")
                {
                    firstPoint = item.Text;
                }
                else if (item.Selected == true && secondPoint == "")
                {
                    secondPoint = item.Text;
                }
                else if (item.Selected == true && secondPoint != "")
                {
                    thirdPoint = item.Text.Substring(0, 1);
                }
            }
            if (secondPoint == "")
            {
                MessageBox.Show("選兩張牌點數相同的牌!");
            }
            else if (thirdPoint != "")
            {
                MessageBox.Show("選取超過兩張牌啦!");
            }
            else
            {
                if (firstPoint.Substring(0, 1) == secondPoint.Substring(0, 1) && firstPoint.Substring(0, 1) != "g")
                {
                    //做刪除重複牌
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = cnstr;
                        cn.Open();
                        string delectStr = $"delete from playerB where points='{firstPoint.Substring(0, 1)}' and suits='{firstPoint.Substring(1, 1)}'";
                        string delectStr2 = $"delete from playerB where points='{secondPoint.Substring(0, 1)}' and suits='{secondPoint.Substring(1, 1)}'";
                        SqlCommand cmd5 = new SqlCommand(delectStr, cn);
                        cmd5.ExecuteNonQuery();
                        SqlCommand cmd6 = new SqlCommand(delectStr2, cn);
                        cmd6.ExecuteNonQuery();
                    }
                    ShowData();
                }
                else if (firstPoint.Substring(0, 1) == secondPoint.Substring(0, 1) && firstPoint.Substring(0, 1) == "g")
                {
                    MessageBox.Show("You Bad Bad.嘖嘖");
                }
                else
                {
                    MessageBox.Show("兩張點數不一樣!!");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("確定要重新一局嗎?", "重玩", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Form1 form1 = new Form1();
                this.Visible = false;
                form1.Visible = true;
            }
            else if (dialogResult == DialogResult.No)
            {

            }
        }
    }
}
