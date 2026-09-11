using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void PageNav(int page)
        {
            switch (page)
            {
                case 1:
                    //userControl11.BringToFront();
                    break;
                case 2:
                    //userControl21.BringToFront();
                    break;
                default:
                    break;
            }
        }

        private void createFactsBtnNav_Click(object sender, EventArgs e)
        {
            PageNav(1);
        }

        private void manageFactsBtnNav_Click(object sender, EventArgs e)
        {
            PageNav(2);
        }
    }
}
