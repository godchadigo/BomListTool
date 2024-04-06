using BomListTool.Manager;
using BomListTool.Models;
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
    public partial class Form2 : DockContent
    {
        private List<Product> chooseProducts = new List<Product>();
        public Form2()
        {
            InitializeComponent();
            InitGrid();
        }

        private void InitGrid()
        {            
            dataGridView1.Columns.Add("產品名稱", "產品名稱");
            dataGridView1.Columns.Add("製造公司", "製造公司");
            dataGridView1.Columns.Add("產品類別", "產品類別");
            dataGridView1.Columns.Add("進料價", "進料價");
            dataGridView1.Columns.Add("銷售價", "銷售價");
        }
        private void Instance_AddProductEvent(string Uuid)
        {
            Console.WriteLine("Form2" + Uuid);
            var res = ProductManager.Instance.Config.ProductList.Where(x => x.Uuid == Uuid).FirstOrDefault();
            if (res != null)
            {
                Console.WriteLine(res.ToString());
                chooseProducts.Add(res);
                //dataGridView1.Rows.Add(res.ProductName , res.GenCompany , res.ProductType , res.ProductOriginPrice , res.ProductSellPrice );
                RefreshGrid();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ProductManager.Instance.AddProductEvent += Instance_AddProductEvent;
        }
        private void RefreshGrid()
        {
            dataGridView1.Rows.Clear();
            var originTotalPrice = 0;
            var sellTotalPrice = 0;
            foreach (var product in chooseProducts)
            {
                originTotalPrice += product.ProductOriginPrice;
                sellTotalPrice += product.ProductSellPrice;
                dataGridView1.Rows.Add(product.ProductName, product.GenCompany, product.ProductType, product.ProductOriginPrice, product.ProductSellPrice);
            }
            
            dataGridView1.Rows.Add("價格統計" , "" , "" , originTotalPrice , sellTotalPrice );
            dataGridView1.Rows[dataGridView1.Rows.Count-2].DefaultCellStyle.BackColor = Color.Yellow;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var rowID = e.RowIndex;
            if (rowID != -1 && rowID < chooseProducts.Count)
            {
                //chooseProducts[rowID]
            }
        }
    }
}
