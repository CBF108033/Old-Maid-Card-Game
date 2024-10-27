using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class Exchange2 : Form
    {
        public Exchange2()
        {
            InitializeComponent();
        }
        string cnstr2 = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DB.mdf;Integrated Security=True";
        //交換
        //bool ischange = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = cnstr2;
                    cn.Open();
                    string sqlcmd = "select * from playerB";
                    SqlDataAdapter da = new SqlDataAdapter(sqlcmd, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "cards");
                    int tb1 = 0;
                    int tb2 = 0;
                    int totalA = 0;
                    if(Int32.TryParse(textBox1.Text, out tb1) == false || Int32.TryParse(textBox2.Text, out tb2) == false)
                    {
                        MessageBox.Show("請輸入數字!");
                    }
                    else
                    {
                        totalA = ds.Tables["cards"].Rows.Count;
                        if (tb1 <= totalA && tb1 > 0 && tb2 <= totalA && tb2 > 0)
                        {
                            //新增tetbox1指的那列資料到最後，當作swap的tem，最後會刪掉
                            string id = ds.Tables["cards"].Rows[tb1 - 1]["id"].ToString();
                            string point = ds.Tables["cards"].Rows[tb1 - 1]["points"].ToString();
                            string suit = ds.Tables["cards"].Rows[tb1 - 1]["suits"].ToString();
                            string joker = ds.Tables["cards"].Rows[tb1 - 1]["joker"].ToString();
                            string id2 = ds.Tables["cards"].Rows[tb2 - 1]["id"].ToString();
                            string point2 = ds.Tables["cards"].Rows[tb2 - 1]["points"].ToString();
                            string suit2 = ds.Tables["cards"].Rows[tb2 - 1]["suits"].ToString();
                            string joker2 = ds.Tables["cards"].Rows[tb2 - 1]["joker"].ToString();

                            if ((point != "" || suit != "" || joker != "") && (point2 != "" || suit2 != "" || joker2 != ""))
                            {
                                //string insertStr = $"insert into playerB(points,suits,joker) values('{point}','{suit}','{joker}')";
                                //SqlCommand cmd2 = new SqlCommand(insertStr, cn);
                                //cmd2.ExecuteNonQuery();

                                string updateStr = $"update playerB set points='{point2}', suits='{suit2}', joker='{joker2}' where id='{id}'";
                                SqlCommand cmd3 = new SqlCommand(updateStr, cn);
                                cmd3.ExecuteNonQuery();
                                string updateStr2 = $"update playerB set points='{point}', suits='{suit}', joker='{joker}' where id='{id2}'";
                                SqlCommand cmd4 = new SqlCommand(updateStr2, cn);
                                cmd4.ExecuteNonQuery();

                                ////將前面做的所有更新後新的資料重新放入資料表cards2
                                //SqlDataAdapter da2 = new SqlDataAdapter(sqlcmd, cn);
                                //da2.Fill(ds, "cards2");
                                //int last = ds.Tables["cards2"].Rows.Count - 1;
                                //string lastId = ds.Tables["cards2"].Rows[last]["id"].ToString();

                                ////刪除最後一筆
                                //string delectStr = $"delete from playerB where id='{lastId}'";
                                //SqlCommand cmd5 = new SqlCommand(delectStr, cn);
                                //cmd5.ExecuteNonQuery();

                                ShowPokers();
                                textBox1.Text = "";
                                textBox2.Text = "";
                            }
                        }
                        else
                        {
                            MessageBox.Show("請輸入要交換的兩張牌(在有效範圍內)");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("請輸入要交換的兩張牌");
            }
        }

        private void Exchange2_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;

            Form1 form1 = new Form1();
            button2.Text = $"換人({form1.playerAname})";
            //button1.Enabled = false;
            //button2.Enabled = false;
            ShowPokers();
            //呈現抽到的牌
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstr2;
                cn.Open();
                string sqlstr = "select * from playerB";
                SqlCommand cmd = new SqlCommand(sqlstr, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string point = "";
                string suit = "";
                while (dr.Read())   //把最後一筆資料 放進兩個變數 用以放入圖片路徑
                {
                    point = dr["points"].ToString().Trim();
                    suit = dr["suits"].ToString().Trim();
                    if(point == "gg")
                    {
                        point = "g";
                    }
                }
                dr.Close();
                pictureBox1.ImageLocation = @"../../poker/" + point.Trim() + suit.Trim() + ".gif";  //選擇圖片位置
            }
        }

        private void ShowPokers()
        {
            //listview顯示玩家A手上牌的圖片
            using (SqlConnection cn = new SqlConnection())
            {
                imageList1.Images.Clear();
                listView1.Items.Clear();
                //preset
                listView1.View = View.LargeIcon;
                imageList1.ImageSize = new System.Drawing.Size(60, 60);
                listView1.LargeImageList = imageList1;
                imageList1.ColorDepth = ColorDepth.Depth32Bit;

                cn.ConnectionString = cnstr2;
                cn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adpterA = new SqlDataAdapter("select * from playerB", cn);
                adpterA.Fill(ds, "playerB");
                DataTable dt = ds.Tables["playerB"];
                string[] pointsArray = new string[dt.Rows.Count];
                string[] suitsArray = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    pointsArray[i] = dt.Rows[i][1].ToString().Trim(' ');
                    suitsArray[i] = dt.Rows[i][2].ToString().Trim(' ');
                    if(pointsArray[i] == "gg")
                    {
                        pointsArray[i] = "g";
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    imageList1.Images.Add(Image.FromFile($"../../poker/{pointsArray[i]}{suitsArray[i]}.gif"));
                }
                listView1.LargeImageList = imageList1;

                for (int j = 0; j < imageList1.Images.Count; j++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = $"{j + 1}";
                    item.ImageIndex = j;
                    this.listView1.Items.Add(item);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstr2;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                SqlDataAdapter da2 = new SqlDataAdapter("select * from playerB", cn);
                DataSet ds = new DataSet();
                da.Fill(ds, "playerA");
                da2.Fill(ds, "playerB");
                Form1 form1 = new Form1();
                if (ds.Tables["playerB"].Rows.Count == 0) 
                {
                    MessageBox.Show($"{form1.playerBname}贏了！");
                    System.Environment.Exit(0);
                }
                if (ds.Tables["playerA"].Rows.Count == 0)
                {
                    MessageBox.Show($"{form1.playerAname}贏了！");
                    System.Environment.Exit(0);
                }
            }
            waiting waiting = new waiting(1);    //要求放入變數
            waiting.Visible = true;
            //if(ischange == true)
            //{
            //    this.Visible = false;
            //    Player1 player1 = new Player1();    //要求放入變數
            //    player1.Visible = true;
            //}
            //else
            //{
            //    MessageBox.Show("請先將成對牌抽出");
            //}

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
                    if (s.Key != "g" || s.Key != "gg")
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
        private void button3_Click(object sender, EventArgs e)
        {
            //button1.Enabled = true;
            //button2.Enabled = true;
            //ischange = true;
            del_Poker();
            ShowPokers();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string firstPoint = "";
            string secondPoint = "";
            string thirdPoint = "";
            int index1 = 0;
            int index2 = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Selected == true && firstPoint == "")
                {
                    firstPoint = item.Text;
                    index1 = item.Index;
                }
                else if (item.Selected == true && secondPoint == "")
                {
                    secondPoint = item.Text;
                    index2 = item.Index;
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
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = cnstr2;
                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from playerB", cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "playerB");
                    if (ds.Tables["playerB"].Rows[index1]["points"].ToString().Trim() == ds.Tables["playerB"].Rows[index2]["points"].ToString().Trim() && ds.Tables["playerB"].Rows[index1]["points"].ToString().Trim() != "g")
                    {
                        index1 = Convert.ToInt32(ds.Tables["playerB"].Rows[index1]["id"]);
                        index2 = Convert.ToInt32(ds.Tables["playerB"].Rows[index2]["id"]);
                        string delectStr = $"delete from playerB where id='{index1}'";
                        string delectStr2 = $"delete from playerB where id='{index2}'";
                        SqlCommand cmd5 = new SqlCommand(delectStr, cn);
                        cmd5.ExecuteNonQuery();
                        SqlCommand cmd6 = new SqlCommand(delectStr2, cn);
                        cmd6.ExecuteNonQuery();
                        ShowPokers();
                    }
                    else if (ds.Tables["playerB"].Rows[index1]["points"].ToString().Substring(0,1) == ds.Tables["playerB"].Rows[index2]["points"].ToString().Substring(0, 1) && ds.Tables["playerB"].Rows[index1]["points"].ToString().Trim().Contains('g') == true)
                    {
                        MessageBox.Show("You Bad Bad.嘖嘖");
                    }
                    else
                    {
                        MessageBox.Show("兩張點數不一樣!!");
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
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
