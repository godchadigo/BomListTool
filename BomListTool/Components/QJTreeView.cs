using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomListTool.Components
{
    public partial class QJTreeView : TreeView
    {
        public QJTreeView()
        {
            InitializeComponent();
        }

        public QJTreeView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
