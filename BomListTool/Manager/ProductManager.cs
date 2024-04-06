using BomListTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomListTool.Manager
{
    public class ProductManager
    {
        public bool isForm3Open { get; set; } = false;
        public static ProductManager Instance { get; private set; } = new ProductManager();
        public BomConfigModel Config {  get; set; }
        public delegate void ProductEvent(string Uuid);
        public event ProductEvent AddProductEvent;
        public delegate void EditEvent();
        public event EditEvent EditFinishEvent;
        public void Init()
        {
            
        }
        public void addProductTrig(string uuid)
        {
            AddProductEvent(uuid);
        }
        public void editProductFinishTrig()
        {
            EditFinishEvent();
        }
        public void addProduct(Product p)
        {
            Config.ProductList.Add(p);
        }
    }
}
