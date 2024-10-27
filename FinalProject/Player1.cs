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
    public partial class Player1 : Form
    {
        public Player1()
        {
            InitializeComponent();
        }
        string cnstr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|DB.mdf;Integrated Security=True";
        void ShowData() //顯示自己的牌
        {
            listView1.Clear(); //清除listView1的值
            imageList1.Images.Clear(); //清除imageList1的值
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                + "Integrated Security=True";
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                DataSet ds = new DataSet();
                da.Fill(ds, "playerA");

                listView1.View = View.LargeIcon; //設定顯示圖片的格式:大的圖示
                imageList1.ImageSize = new System.Drawing.Size(60, 60); //圖片的大小是64x64
                listView1.LargeImageList = imageList1; //把imagelist放到listview
                imageList1.ColorDepth = ColorDepth.Depth32Bit; //imagelist顯示的色彩數目

                for (int i = 0; i < ds.Tables["playerA"].Rows.Count; i++) //顯示playerA的牌,
                //因為會隨著抽牌減少所以用rows判斷資料列大小
                {
                    string points = ds.Tables["playerA"].Rows[i]["points"].ToString(); //points的字串
                    points = points.Replace(" ", String.Empty); //從SQL字串轉成C#字串
                    string suits = ds.Tables["playerA"].Rows[i]["suits"].ToString();
                    suits = suits.Replace(" ", String.Empty);
                    if(points == "gg")
                    {
                        points = "g";
                    }
                    string test = "../../poker/" + points + suits + ".gif"; //圖片的位置
                    imageList1.Images.Add(Image.FromFile(test)); //imageList建立圖片
                    listView1.Items.Add(Path.GetFileName(test)); //新增圖片的位置
                    listView1.Items[i].ImageIndex = i; //顯示每張牌的影像
                }
            }
        }

        void del_Poker() //刪除成對牌的方法
        {
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;"
                + @"AttachDbFilename=|DataDirectory|DB.mdf;"
                + "Integrated Security=True";
                cn.Open();
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter("select * from playerA", cn);
                da.Fill(ds, "playerA");
                string[] point = new string[ds.Tables["playerA"].Rows.Count]; 
                //建立存放points的陣列
                for(int i=0;i<point.Length;i++)
                {
                    point[i] = ds.Tables["playerA"].Rows[i]["points"].ToString().Trim(); 
                    //把資料表points的資料一筆一筆存入
                }
                
                //LINQ
                foreach (var s in point.GroupBy(c => c)) //統計points重複出現的次數
                {
                    if(s.Key != "g" || s.Key != "gg") //如果point不等於鬼牌才做刪除動作(鬼牌是gg.gif)
                    {
                        if(s.Count()%2==0 && s.Count()>1) 
                            //如果這個點數出現次數是偶數而且大於1張
                        {
                            string cmdstr = $"delete From playerA WHERE points ='{s.Key}'";
                            //把這個點數的牌都刪除
                            SqlCommand cmd = new SqlCommand(cmdstr, cn);
                            cmd.ExecuteNonQuery();
                        }
                        else if(s.Count()%2!=0 && s.Count()>1)
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
        int count = 0;
        public static int gameTime = 0;
        public int gametime2 = gameTime;

        
        private void Player1_Load(object sender, EventArgs e)
        {

            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;

            Form1 form1 = new Form1();
            label2.Text = $"我的牌({form1.playerAname}) :";
            label3.Text = $"對方的牌({form1.playerBname}) :";
            Player2 player2 = new Player2();
            //判斷 抽出一對牌 要不要顯示
            //只有第一個先開始的人需要顯示 抽出一對牌
            if (player2.gametime2 > 0)  //不是第一個的話
            {
                button1.Visible = false;    //隱藏自動抽出成對牌
                button3.Visible = false;    //隱藏手動抽出成對牌
            }
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

                ShowData(); //顯示自己的牌

                SqlCommand cmdB = new SqlCommand(selectBstr, cn);
                SqlDataReader dr = cmdB.ExecuteReader();

                while (dr.Read())
                {      
                    count += 1;
                }
                pictureBox1.ImageLocation = @"../../pokerBack/" + count + ".png";
                dr.Close();
                cn.Close();
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
                    string sqlstr = "select * from playerB";
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
                        count2 += 1;
                    }
                }
                if (points != "" || suits != "" || joker != "")
                {
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = cnstr;
                        cn.Open();
                        //insert抽到的牌至A
                        string insertStr = $"insert into playerA(points,suits,joker) values('{points}','{suits}','{joker}')";
                        SqlCommand cmd2 = new SqlCommand(insertStr, cn);
                        cmd2.ExecuteNonQuery();
                        //從B delete被抽到的牌 
                        string delectStr = $"delete from playerB where id='{id}'";
                        SqlCommand cmd3 = new SqlCommand(delectStr, cn);
                        cmd3.ExecuteNonQuery();
                    }
                    this.Visible = false;
                    Exchange1 exchange1 = new Exchange1();
                    exchange1.Visible = true;
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
            del_Poker();
            ShowData();
            //刪除牌後再顯示
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
                else if(item.Selected == true && secondPoint == "")
                {
                    secondPoint = item.Text;
                }
                else if(item.Selected == true && secondPoint != "")
                {
                    thirdPoint= item.Text.Substring(0, 1);
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
                        string delectStr = $"delete from playerA where points='{firstPoint.Substring(0, 1)}' and suits='{firstPoint.Substring(1, 1)}'";
                        string delectStr2 = $"delete from playerA where points='{secondPoint.Substring(0, 1)}' and suits='{secondPoint.Substring(1, 1)}'";
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
