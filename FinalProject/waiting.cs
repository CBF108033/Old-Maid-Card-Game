using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class waiting : Form
    {
        int orNot = 0;
        public waiting(int AorB_num)
        {
            orNot = AorB_num;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player1 player1 = new Player1();
            Player2 player2 = new Player2();
            if (orNot == 1)
            {
                player1.Visible = true;
                player2.Visible = false;
                this.Visible = false;
            }
            else if (orNot == 2)
            {
                player1.Visible = false;
                player2.Visible = true;
                this.Visible = false;
            }
        }

        private void waiting_Load(object sender, EventArgs e)
        {
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + @"\Skins\Emerald.ssk";
            this.skinEngine1.Active = true;
        }
    }
}
