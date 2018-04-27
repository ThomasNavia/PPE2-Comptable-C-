using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPE2_Comptable
{
    public partial class Menu : Form
    {
        string IpAddr; 
        public Menu(string addr)
        {
            InitializeComponent();
            IpAddr = addr; 
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Validation op = new Validation(IpAddr);
            op.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Connexion VueConnexion = new Connexion();
            VueConnexion.Show(); 
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Remboursement VueRemboursement = new Remboursement(IpAddr);
            VueRemboursement.Show(); 
        }
    }
}
