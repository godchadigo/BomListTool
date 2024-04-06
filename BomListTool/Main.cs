using BomListTool.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace BomListTool
{
    public partial class Main : Form
    {
        //1.0.2 新增供應商，以及進價時間
        public string Version = "1.0.2";
        public Main()
        {
            InitializeComponent();
            var ProductForm = new Form1();
            var WorkForm = new Form2();

            ProductForm.Show(dockPanel1, dockState: WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            WorkForm.Show(dockPanel1, dockState: WeifenLuo.WinFormsUI.Docking.DockState.Document);
        }


        
        private void 開啟編輯器ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {            
            // 先检查是否已打开 Form3 的实例
            foreach (var content in dockPanel1.Contents)
            {
                if (content is Form3)
                {
                    // Form3 已经打开
                    ProductManager.Instance.isForm3Open = true;
                    // 可以选择将已打开的窗口带到前台
                    ((Form3)content).Activate();
                    break; // 找到后就可以退出循环了
                }
            }

            // 如果 Form3 没有打开，则创建并显示一个新的实例
            if (!ProductManager.Instance.isForm3Open)
            {
                Form3 f3 = new Form3();
                f3.Show(dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            }

        }
    }
}
