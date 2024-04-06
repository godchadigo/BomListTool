using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomListTool.Dialog
{
    public enum ChooseStatusEnum
    {
        Yes,
        No,
        Cancel,
    }
    
    public partial class threeProblem : Form
    {
        public string ExFormText { get; set; } = "";
        public string ExProblemText { get; set; }
        public ChooseStatusEnum ExchooseStatus { get; set; }

        public threeProblem()
        {
            InitializeComponent();
        }

        private void threeProblem_Load(object sender, EventArgs e)
        {
            this.Text = ExFormText;

        }
    }
}
