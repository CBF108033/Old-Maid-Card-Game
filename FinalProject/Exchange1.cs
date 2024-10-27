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
    public partial class Exchange1 : Form
    {
        public Exchange1()
        {
            InitializeComponent();
        }
        string cnstr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DB.mdf;Integrated Security=True";
        //交換
        //bool ischange = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = cnstr;
                    cn.Open();
                    string sqlcmd = "select * from playerA";
                    SqlDataAdapter da = new SqlDataAdapter(sqlcmd, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "cards");
                    int tb1 = 0;
                    int tb2 = 0;
                    int totalA = 0;
                    if (Int32.TryParse(textBox1.Text, out tb1) == false || Int32.TryParse(textBox2.Text, out tb2) == false)
                    {
                        MessageBox.Show("請輸入數字!");
                    }
                    else
                    {
                        totalA = ds.Tables["cards"].Rows.Count;
                        if (tb1 <= totalA && tb1 > 0 && tb2 <= totalA && tb2 > 0)
                        {
                            ////新增tetbox1指的那列資料到最後，當作swap的tem，最後會刪掉
                            //SqlCommand cmd = new SqlCommand("insert", cn);
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.Add(new SqlParameter("@point", SqlDbType.NChar));
                            //cmd.Parameters.Add(new SqlParameter("@suit", SqlDbType.NChar));
                            //cmd.Parameters.Add(new SqlParameter("@joker", SqlDbType.NChar));
                            //cmd.Parameters["@point"].Value = ds.Tables["cards"].Rows[tb1 - 1]["points"].ToString().Trim();
                            //cmd.Parameters["@suit"].Value = ds.Tables["cards"].Rows[tb1 - 1]["suits"].ToString().Trim();
                            //cmd.Parameters["@joker"].Value = ds.Tables["cards"].Rows[tb1 - 1]["joker"].ToString().Trim();
                            //cmd.ExecuteNonQuery();

                            //將textbox1所指的row 資料更新成texrbox2所指的row之資料
                            SqlCommand cmd2 = new SqlCommand("update", cn);
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.Add(new SqlParameter("@point", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@suit", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@joker", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                            cmd2.Parameters["@point"].Value = ds.Tables["cards"].Rows[tb2 - 1]["points"].ToString().Trim();
                            cmd2.Parameters["@suit"].Value = ds.Tables["cards"].Rows[tb2 - 1]["suits"].ToString().Trim();
                            cmd2.Parameters["@joker"].Value = ds.Tables["cards"].Rows[tb2 - 1]["joker"].ToString().Trim();
                            cmd2.Parameters["@id"].Value = ds.Tables["cards"].Rows[tb1 - 1]["id"].ToString().Trim();
                            cmd2.ExecuteNonQuery();
                            //清空 cmd2 ADD資料，在重建給第二次update
                            //將textbox2所指的row 資料更新成texrbox1所指的row之資料
                            cmd2.Parameters.Clear();
                            cmd2.Parameters.Add(new SqlParameter("@point", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@suit", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@joker", SqlDbType.NChar));
                            cmd2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                            cmd2.Parameters["@point"].Value = ds.Tables["cards"].Rows[tb1 - 1]["points"].ToString().Trim();
                            cmd2.Parameters["@suit"].Value = ds.Tables["cards"].Rows[tb1 - 1]["suits"].ToString().Trim();
                            cmd2.Parameters["@joker"].Value = ds.Tables["cards"].Rows[tb1 - 1]["joker"].ToString().Trim();
                            cmd2.Parameters["@id"].Value = ds.Tables["cards"].Rows[tb2 - 1]["id"].ToString().Trim();
                            cmd2.ExecuteNonQuery();

                            ////將前面做的所有更新後 新的資料 重新放入資料表cards2
                            //SqlDataAdapter da2 = new SqlDataAdapter(sqlcmd, cn);
                            //da2.Fill(ds, "cards2");
                            //int last = ds.Tables["cards2"].Rows.Count - 1;
                            //string lastId = ds.Tables["cards2"].Rows[last]["id"].ToString().Trim();

                            ////刪除最後一筆
                            //string delectStr = $"delete from playerA where id='{lastId}'";
                            //SqlCommand cmd5 = new SqlCommand(delectStr, cn);
                            //cmd5.ExecuteNonQuery();

                            ShowPokers();
                            textBox1.Text = "";
                            textBox2.Text = "";
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

        private void Exchange1_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;


            Form1 form1 = new Form1();
            button2.Text = $"換人({form1.playerBname})";
            ShowPokers();
            //呈現抽到的牌
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstr;
                cn.Open();
                string sqlstr = "select * from playerA";
                SqlCommand cmd = new SqlCommand(sqlstr, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string point = "";
                string suit = "";
                while (dr.Read())   //把最後一筆資料 放進兩個變數 用以放入圖片路徑
                {
                    point = dr["points"].ToString().Trim();
                    suit = dr["suits"].ToString().Trim();
                }
                dr.Close();
                if (point == "gg")
                {
                    point = "g";
                }
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

                cn.ConnectionString = cnstr;
                cn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter adpterA = new SqlDataAdapter("select * from playerA", cn);
                adpterA.Fill(ds, "playerA");
                DataTable dt = ds.Tables["playerA"];
                string[] pointsArray = new string[dt.Rows.Count];
                string[] suitsArray = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    pointsArray[i] = dt.Rows[i][1].ToString().Trim();
                    suitsArray[i] = dt.Rows[i][2].ToString().Trim();
                    if (pointsArray[i] == "gg")
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
                cn.ConnectionString = cnstr;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                SqlDataAdapter da2 = new SqlDataAdapter("select * from playerB", cn);
                DataSet ds = new DataSet();
                da.Fill(ds, "playerA");
                da2.Fill(ds, "playerB");
                Form1 form1 = new Form1();
                if (ds.Tables["playerA"].Rows.Count == 0)
                {
                    MessageBox.Show($"{form1.playerAname}贏了！");
                    System.Environment.Exit(0);
                }
                else if (ds.Tables["playerB"].Rows.Count == 0)
                {
                    MessageBox.Show($"{form1.playerBname}贏了!");
                    System.Environment.Exit(0);
                }
            }
            waiting waiting = new waiting(2);    //要求放入變數
            waiting.Visible = true;
        }
        void del_Poker() //刪除成對牌的方法
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstr;
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
                foreach (var s in point.GroupBy(c => c)) //統計points重複出現的次數
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
                    cn.ConnectionString = cnstr;
                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "playerA");
                    if (ds.Tables["playerA"].Rows[index1]["points"].ToString().Trim() == ds.Tables["playerA"].Rows[index2]["points"].ToString().Trim() && ds.Tables["playerA"].Rows[index1]["points"].ToString().Trim() != "g")
                    {
                        index1 = Convert.ToInt32(ds.Tables["playerA"].Rows[index1]["id"]);
                        index2 = Convert.ToInt32(ds.Tables["playerA"].Rows[index2]["id"]);
                        string delectStr = $"delete from playerA where id='{index1}'";
                        string delectStr2 = $"delete from playerA where id='{index2}'";
                        SqlCommand cmd5 = new SqlCommand(delectStr, cn);
                        cmd5.ExecuteNonQuery();
                        SqlCommand cmd6 = new SqlCommand(delectStr2, cn);
                        cmd6.ExecuteNonQuery();
                        ShowPokers();
                    }
                    else if (ds.Tables["playerA"].Rows[index1]["points"].ToString().Substring(0, 1) == ds.Tables["playerA"].Rows[index2]["points"].ToString().Substring(0, 1) && ds.Tables["playerA"].Rows[index1]["points"].ToString().Trim().Contains('g') == true)
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
