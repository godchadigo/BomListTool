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
using System.Dynamic;

namespace BomListTool
{
    public partial class Form1 : DockContent
    {
        List<DockContent> dc = new List<DockContent>();
        
        public Form1()
        {
            InitializeComponent();
            LoadConfig();
            dataGridView1.Columns.Add("屬性" , "屬性");
            dataGridView1.Columns.Add("數值", "數值");
            dataGridView1.Visible = false;
            
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var config = new BomConfigModel();            
            config.ProductList = new List<Product>() { 
                new Product()
                {
                    GenCompany = "士林",
                    ProductType = "工業材料",
                    ProductName = "MC-123",
                    ProductGroup = "電磁接觸器",
                    ProductOriginPrice = 192,
                    ProductSellPrice = 200,                    
                },
                new Product()
                {
                    GenCompany = "士林",
                    ProductType = "工業材料",
                    ProductName = "MC-1123",
                    ProductGroup = "電磁接觸器",
                    ProductOriginPrice = 1912,
                    ProductSellPrice = 2007,
                },
                new Product()
                {
                    GenCompany = "士林",
                    ProductType = "工業材料",
                    ProductName = "MC-1253",
                    ProductGroup = "電磁接觸器",
                    ProductOriginPrice = 1292,
                    ProductSellPrice = 3200,
                },
                new Product()
                {
                    GenCompany = "台達",
                    ProductType = "工業材料",
                    ProductName = "DMC-1213",
                    ProductGroup = "電磁接觸器",
                    ProductOriginPrice = 3192,
                    ProductSellPrice = 5200,
                },
                new Product()
                {
                    GenCompany = "台達",
                    ProductType = "工業材料",
                    ProductName = "DMC-1243",
                    ProductGroup = "電磁接觸器",
                    ProductOriginPrice = 1792,
                    ProductSellPrice = 2000,
                },
                 new Product()
                {
                    GenCompany = "士林",
                    ProductType = "工業材料",
                    ProductName = "SDP-E",
                    ProductGroup = "AC伺服馬達",
                    ProductOriginPrice = 9792,
                    ProductSellPrice = 1000,
                    CustomProperty = new List<CustomPropertyModel>(){ 
                        new CustomPropertyModel(){ 
                            Key = "電壓",
                            Value = "110v",
                        },
                        new CustomPropertyModel(){
                            Key = "電流",
                            Value = "43a",
                        },
                        new CustomPropertyModel(){
                            Key = "轉矩",
                            Value = "50nm",
                        },
                    },
                    Description = "這是一個範例",
                },
            };
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(exeFilePath);

            var res = Newtonsoft.Json.JsonConvert.SerializeObject(config , Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(directoryPath + "\\"+ "Config.qjbom", res);
        }
#if true
        private void button2_Click(object sender, EventArgs e)
        {

            LoadConfig();
        }
#endif
        private void LoadConfig()
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(exeFilePath);
            var content = File.ReadAllText(directoryPath + "\\" + "Config.qjbom");
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<BomConfigModel>(content);

            ProductManager.Instance.Config = res;

            // 使用 LINQ 分类和组合数据

            var productList = ProductManager.Instance.Config.ProductList;

            var groupedProducts = productList
                .GroupBy(p => p.ProductType)
                .Select(g => new
                {
                    ProductType = g.Key,
                    ProductGroups = g.GroupBy(p => p.ProductGroup)
                                     .Select(pg => new
                                     {
                                         ProductGroup = pg.Key,
                                         Companies = pg.GroupBy(p => p.GenCompany)
                                                        .Select(gc => new
                                                        {
                                                            GenCompany = gc.Key,
                                                            Products = gc.OrderBy(p => p.ProductName)
                                                                         .Select(p => new
                                                                         {
                                                                             p.Uuid,
                                                                             p.GenCompany,
                                                                             p.ProductGroup,
                                                                             p.ProductType,
                                                                             p.ProductName,
                                                                             p.ProductOriginPrice,
                                                                             p.ProductSellPrice,
                                                                             p.SourceCompany,
                                                                             p.CreateDate,
                                                                             // 在这里添加更多需要的属性
                                                                         })
                                                        })
                                     })
                });


            qjTreeView1.Nodes.Clear();
            // 打印分组结果
            foreach (var typeGroup in groupedProducts)
            {
                Console.WriteLine($"ProductType: {typeGroup.ProductType}");
                var Node1 = qjTreeView1.Nodes.Add(typeGroup.ProductType);
                foreach (var group in typeGroup.ProductGroups)
                {
                    Console.WriteLine($"  ProductGroup: {group.ProductGroup}");
                    var Node2 = Node1.Nodes.Add(group.ProductGroup);
                    foreach (var company in group.Companies)
                    {
                        Console.WriteLine($"    GenCompany: {company.GenCompany}");
                        var Node3 = Node2.Nodes.Add(company.GenCompany);

                        foreach (var product in company.Products)
                        {
                            

                            // 如果你有 UUID 和你想要附加到某个节点或其他对象
                            // 你可以这样获取 UUID
                            // var uuid = (Guid)product["Uuid"];
                            TreeNode Node4;
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
                            Node4 = Node3.Nodes.Add($"{product.ProductName}{srcCommpany}");
                            Node4.Tag = product.Uuid.ToString();
                        }
                    }
                }
            }
        }
        private void qjTreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = e.Node.Tag;
            if (tag == null) return;
            if (tag.ToString().Length == 0) return;
            ProductManager.Instance.addProductTrig(tag.ToString());
            Console.WriteLine(tag);
        }

        private void qjTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tag = e.Node.Tag;
            if (tag == null) return;
            var res = ProductManager.Instance.Config.ProductList.Where(p => p.Uuid == tag.ToString()).FirstOrDefault();
            if (res != null)
            {
                textBox1.Text = res?.ProductName;
                textBox2.Text = res?.GenCompany;
                textBox3.Text = res?.ProductType;
                textBox4.Text = res.ProductOriginPrice.ToString();
                textBox5.Text = res.ProductSellPrice.ToString();
                textBox6.Text = res?.SourceCompany;
                if (res?.CreateDate.Day == 1)
                {
                    textBox7.Text = "時間未定義";
                }
                else
                {
                    textBox7.Text = res?.CreateDate.ToString("yyyy-MM-dd");
                }
                

                richTextBox1.Text = res.Description;

                if (res.Img != null)
                {
                    if (res.Img.Length != 0)
                    {
                        ImageHelper.DisplayBase64InPictureBox(res.Img, pictureBox1);
                    }
                    else
                    {
                        pictureBox1.Image = null;   
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
                
                dataGridView1.Visible = false;
                if (res.CustomProperty != null)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Visible = true;
                    
                    foreach (var cProperty in res.CustomProperty)
                    {
                        dataGridView1.Rows.Add(cProperty.Key, cProperty.Value);
                    }
                }
            }                                   
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        private static object GetProperties(object obj)
        {
            var props = obj.GetType().GetProperties();
            var propValues = new Dictionary<string, object>();

            foreach (var prop in props)
            {
                propValues.Add(prop.Name, prop.GetValue(obj, null));
            }

            return propValues; // 返回一个属性名称到属性值的映射
        }

        private void qjTreeView1_Move(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProductManager.Instance.EditFinishEvent += Instance_EditFinishEvent;
        }
        /// <summary>
        /// 編輯器編輯完成事件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Instance_EditFinishEvent()
        {
            // 使用 LINQ 分类和组合数据

            var productList = ProductManager.Instance.Config.ProductList;

            var groupedProducts = productList
                .GroupBy(p => p.ProductType)
                .Select(g => new
                {
                    ProductType = g.Key,
                    ProductGroups = g.GroupBy(p => p.ProductGroup)
                                     .Select(pg => new
                                     {
                                         ProductGroup = pg.Key,
                                         Companies = pg.GroupBy(p => p.GenCompany)
                                                        .Select(gc => new
                                                        {
                                                            GenCompany = gc.Key,
                                                            Products = gc.OrderBy(p => p.ProductName)
                                                                         .Select(p => new
                                                                         {
                                                                             p.Uuid,
                                                                             p.GenCompany,
                                                                             p.ProductGroup,
                                                                             p.ProductType,
                                                                             p.ProductName,
                                                                             p.ProductOriginPrice,
                                                                             p.ProductSellPrice,
                                                                             p.SourceCompany,
                                                                             p.CreateDate,
                                                                             // 在这里添加更多需要的属性
                                                                         })
                                                        })
                                     })
                });


            qjTreeView1.Nodes.Clear();
            // 打印分组结果
            foreach (var typeGroup in groupedProducts)
            {
                Console.WriteLine($"ProductType: {typeGroup.ProductType}");
                var Node1 = qjTreeView1.Nodes.Add(typeGroup.ProductType);
                foreach (var group in typeGroup.ProductGroups)
                {
                    Console.WriteLine($"  ProductGroup: {group.ProductGroup}");
                    var Node2 = Node1.Nodes.Add(group.ProductGroup);
                    foreach (var company in group.Companies)
                    {
                        Console.WriteLine($"    GenCompany: {company.GenCompany}");
                        var Node3 = Node2.Nodes.Add(company.GenCompany);

                        foreach (var product in company.Products)
                        {


                            // 如果你有 UUID 和你想要附加到某个节点或其他对象
                            // 你可以这样获取 UUID
                            // var uuid = (Guid)product["Uuid"];
                            TreeNode Node4;
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
                            Node4 = Node3.Nodes.Add($"{product.ProductName}{srcCommpany}");
                            Node4.Tag = product.Uuid.ToString();
                        }
                    }
                }
            }
        }
    }
}
