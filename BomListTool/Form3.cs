using BomListTool.Manager;
using BomListTool.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using BomListTool.Dialog;
using System.Text.RegularExpressions;

namespace BomListTool
{
    public partial class Form3 : DockContent
    {
        private string NowChooseUuid = "";
        public Form3()
        {
            InitializeComponent();
            if (RefreshTreeView())
            {
                cbInit();
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshTreeView();
        }

        public bool RefreshTreeView()
        {
            // 使用 LINQ 分类和组合数据

            if (ProductManager.Instance.Config == null) return false;
            qjTreeView1.Nodes.Clear();

            var productList = ProductManager.Instance.Config.ProductList;
            
            var groupedProducts = productList
             .GroupBy(p => p.GenCompany)
             .Select(gCompany => new
             {
                 GenCompany = gCompany.Key,
                 ProductTypes = gCompany
                                 .GroupBy(p => p.ProductType)
                                 .Select(gType => new
                                 {
                                     ProductType = gType.Key,
                                     ProductGroups = gType
                                                     .GroupBy(p => p.ProductGroup)
                                                     .Select(gGroup => new
                                                     {
                                                         ProductGroup = gGroup.Key,
                                                         Products = gGroup.OrderBy(p => p.ProductName)
                                                     })
                                 })
             });

            foreach (var company in groupedProducts)
            {
                var companyNode = qjTreeView1.Nodes.Add(company.GenCompany);
                foreach (var type in company.ProductTypes)
                {
                    var typeNode = companyNode.Nodes.Add(type.ProductType);
                    foreach (var group in type.ProductGroups)
                    {
                        var groupNode = typeNode.Nodes.Add(group.ProductGroup);
                        foreach (var product in group.Products)
                        {
                            TreeNode finalNode;
                            string srcCommpany = "";
                            if (product.SourceCompany != null)
                            {
                                if (product.SourceCompany.ToString().Length != 0)
                                {

                                    if (product.CreateDate.Day == 1)
                                    {
                                        srcCommpany = $"({product.SourceCompany})";
                                    }
                                    else
                                    {
                                        srcCommpany = $"({product.SourceCompany}{product.CreateDate.ToString("yyyy/MM/dd")})";
                                    }

                                }
                                else
                                {
                                    srcCommpany = "";
                                }
                            }
                            else
                            {
                                srcCommpany = "";
                            }
                            finalNode = groupNode.Nodes.Add(product.ProductName + srcCommpany);
                            finalNode.Tag = product.Uuid;
                        }
                    }
                }
            }

            qjTreeView1.ExpandAll(); // Optionally, expand all nodes for visibility

            return true;
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            NowChooseUuid = "";
            //if (NowChooseUuid == "") return;
            if (!IsStringNumeric(textBox1.Text) || !IsStringNumeric(textBox2.Text))
            {
                MessageBox.Show("價格請輸入純數字!");
                return;
            }


            string srcCompany = cbSrcCompany.Text;
            string genCompany = cbGenCompany.Text;
            string productType = cbProductType.Text;
            string productGroup = cbProductGroup.Text;

            var productName = textBox4.Text;
            var productOriginPrice = int.Parse(textBox1.Text);
            var productSellPrice = int.Parse(textBox2.Text);
            var description = richTextBox1.Text;
            var createDate = dateTimePicker1.Value;

            var imgStr = "";
            if (pictureBox1.Image != null)
            {
                imgStr = ImageHelper.ConvertImageToBase64(pictureBox1.Image);
            }


            List<CustomPropertyModel> cPropertyList = new List<CustomPropertyModel>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    cPropertyList.Add(new CustomPropertyModel()
                    {
                        Key = row.Cells[0].Value.ToString(),
                        Value = row.Cells[1].Value.ToString(),
                    });
                }
            }

            ProductManager.Instance.addProduct(new Product()
            {
                ProductType = productType,
                ProductGroup = productGroup,
                ProductName = productName,
                ProductOriginPrice = productOriginPrice,
                ProductSellPrice = productSellPrice,
                SourceCompany = srcCompany,
                GenCompany = genCompany,
                CustomProperty = cPropertyList,
                Img = imgStr,
                Description = description,
                CreateDate = createDate,
            });
            //刷新選擇器表格
            ProductManager.Instance.editProductFinishTrig();
            RefreshTreeView();
            cbInit();   //如果使用者有新增自訂一參數，則刷新到下搭選單中
        }

 
        private void cbInit()
        {
            var productList = ProductManager.Instance.Config.ProductList;
            //var p0 = productList.Select(p => p.SourceCompany).Distinct().ToList();
            var p0 = productList
            .Select(p => p.SourceCompany)
            .Where(s => !string.IsNullOrEmpty(s)) // 添加这行来排除null和空字符串
            .Distinct()
            .ToList();
            
            p0.Add("自定義");
            var p1 = productList.Select(p => p.GenCompany).Distinct().ToList();
            p1.Add("自定義");
            var p2 = productList.Select(p => p.ProductType).Distinct().ToList();
            p2.Add("自定義");
            var p3 = productList.Select(p => p.ProductGroup).Distinct().ToList();
            p3.Add("自定義");

            cbSrcCompany.DataSource = p0;
            cbGenCompany.DataSource = p1;
            cbProductType.DataSource = p2;
            cbProductGroup.DataSource = p3;
        }

        #region SelectIndexChanged
        private void cbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cbProductType.SelectedIndex;
            if (index == -1) return;
            var str = cbProductType.Items[index].ToString();
            if (str == "自定義")
            {
                cbProductType.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cbProductType.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void cbGenCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cbGenCompany.SelectedIndex;
            if (index == -1) return;
            var str = cbGenCompany.Items[index].ToString();
            if (str == "自定義")
            {
                cbGenCompany.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cbGenCompany.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            Console.WriteLine("索引" + index);
        }

        private void cbProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cbProductGroup.SelectedIndex;
            if (index == -1) return;
            var str = cbProductGroup.Items[index].ToString();
            if (str == "自定義")
            {
                cbProductGroup.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cbProductGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        private void cbSrcCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cbSrcCompany.SelectedIndex;
            if (index == -1) return;
            var str = cbSrcCompany.Items[index].ToString();
            if (str == "自定義")
            {
                cbSrcCompany.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                cbSrcCompany.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(exeFilePath);

            var res = Newtonsoft.Json.JsonConvert.SerializeObject(ProductManager.Instance.Config, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(directoryPath + "\\" + "Config.qjbom", res);
        }
        /// <summary>
        /// 刪除選中的產品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (qjTreeView1.SelectedNode == null) return;
            var chooseTag = qjTreeView1.SelectedNode.Tag;
            Console.WriteLine(chooseTag);
            if (chooseTag == null) return;
            var itemToRemove = ProductManager.Instance.Config.ProductList.FirstOrDefault(x => x.Uuid == chooseTag.ToString());
            if (itemToRemove != null)
            {
                var msg = MessageBox.Show("刪除商品?" , $"是否刪除{itemToRemove.ProductName}" , MessageBoxButtons.YesNo);
                if (msg == DialogResult.Yes)
                {
                    ProductManager.Instance.Config.ProductList.Remove(itemToRemove);
                    //刷新選擇器表格
                    ProductManager.Instance.editProductFinishTrig();
                    RefreshTreeView();
                }                
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            InitGrid();
        }

        private void InitGrid()
        {
            dataGridView1.Columns.Add("屬性", "屬性");
            dataGridView1.Columns.Add("數值", "數值");

        }

        private void qjTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = e.Node.Tag;
            if (tag == null) return;
            var res = ProductManager.Instance.Config.ProductList.Where(p => p.Uuid == tag.ToString()).FirstOrDefault();
            if (res != null)
            {

                NowChooseUuid = res.Uuid;

                cbGenCompany.Text = res.GenCompany;
                cbProductType.Text = res.ProductType;
                cbProductGroup.Text = res.ProductGroup;
                if (res.SourceCompany == "" || res.SourceCompany == null)
                {
                    cbSrcCompany.DropDownStyle = ComboBoxStyle.DropDown;
                    cbSrcCompany.Text = "";
                }
                else
                {
                    cbSrcCompany.Text = res.SourceCompany;
                }
                
                textBox4.Text = res.ProductName;
                textBox1.Text = res.ProductOriginPrice.ToString();
                textBox2.Text = res.ProductSellPrice.ToString();
                if (res.CreateDate.Day == 1)
                {
                    dateTimePicker1.Value = DateTime.Now;
                    
                }
                else
                {
                    dateTimePicker1.Value = res.CreateDate;
                    
                }

                if (res.Description != null)
                {
                    richTextBox1.Text = res.Description;
                }
                

                var imgStr = res.Img;
                if (imgStr != null && imgStr.Length != 0)
                {
                    ImageHelper.DisplayBase64InPictureBox(imgStr, pictureBox1);
                }
                else
                {
                    pictureBox1.Image = null;
                }
                
                
                dataGridView1.Rows.Clear();
                if (res.CustomProperty != null)
                {                                        
                    foreach (var cProperty in res.CustomProperty)
                    {
                        dataGridView1.Rows.Add(cProperty.Key, cProperty.Value);
                    }
                }
            }
            else
            {
                NowChooseUuid = "";
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            if (NowChooseUuid == "") return;
            if (!IsStringNumeric(textBox1.Text) || !IsStringNumeric(textBox2.Text))
            {
                MessageBox.Show("價格請輸入純數字!");
                return;
            }


            string srcCompany = cbSrcCompany.Text;
            string genCompany = cbGenCompany.Text;
            string productType = cbProductType.Text;
            string productGroup = cbProductGroup.Text;
          
            var productName = textBox4.Text;
            var productOriginPrice = int.Parse(textBox1.Text);
            var productSellPrice = int.Parse(textBox2.Text);
            var description = richTextBox1.Text;
            var createDate = dateTimePicker1.Value;

            var imgStr = "";
            if (pictureBox1.Image != null)
            {
                 imgStr = ImageHelper.ConvertImageToBase64(pictureBox1.Image);
            }
            

            List<CustomPropertyModel> cPropertyList = new List<CustomPropertyModel>();
            foreach (DataGridViewRow row  in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    cPropertyList.Add(new CustomPropertyModel()
                    {
                        Key = row.Cells[0].Value.ToString(),
                        Value = row.Cells[1].Value.ToString(),
                    });
                }                
            }

            var res = ProductManager.Instance.Config.ProductList.Where(x => x.Uuid == NowChooseUuid).FirstOrDefault();
            if (res != null)
            {                
                res.ProductType = productType;
                res.ProductGroup = productGroup;
                res.ProductName = productName;
                res.ProductOriginPrice = productOriginPrice;
                res.ProductSellPrice = productSellPrice;
                res.SourceCompany = srcCompany;
                res.GenCompany = genCompany;
                res.CustomProperty = cPropertyList;
                res.Img = imgStr;
                res.Description = description;
                res.CreateDate = createDate;
                
                //刷新選擇器表格
                ProductManager.Instance.editProductFinishTrig();
                RefreshTreeView();
            }
            cbInit();   //如果使用者有新增自訂一參數，則刷新到下搭選單中
        }

        /// <summary>
        /// 檢查是否為純數字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsStringNumeric(string input)
        {
            // 正则表达式，用于匹配一个或多个数字
            Regex regex = new Regex(@"^\d+$");

            // 使用正则表达式的IsMatch方法来检查字符串是否匹配
            return regex.IsMatch(input);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (NowChooseUuid == "") return;
            OpenFileDialog dlg = new OpenFileDialog();
            

            // 设置文件过滤器
            dlg.Filter = "Image files (*.png;*.jpg;*.ico)|*.png;*.jpg;*.ico";
            dlg.FilterIndex = 1; // 默认显示第一个过滤器选项
            dlg.RestoreDirectory = true; // 打开对话框后恢复原目录

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // 获取选中文件的路径
                string filePath = dlg.FileName;
                Console.WriteLine($"Selected file: {filePath}");
                // 在这里添加代码处理选中的文件
                var base64Str = ImageHelper.ConvertImageToBase64(filePath);

                var res = ProductManager.Instance.Config.ProductList.Where(x => x.Uuid == NowChooseUuid).FirstOrDefault();
                if (res != null)
                {
                    res.Img = base64Str;                    
                }
                ImageHelper.DisplayBase64InPictureBox(base64Str , pictureBox1);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = null;   

        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }
    }
}
