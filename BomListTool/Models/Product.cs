using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomListTool.Models
{
    public class Product
    {
        public string Uuid {  get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 廠牌
        /// </summary>
        public string GenCompany { get; set; }
        /// <summary>
        /// 供應商
        /// </summary>
        public string SourceCompany {  get; set; }
        public string ProductGroup { get; set; }
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public int ProductOriginPrice { get; set; }
        public int ProductSellPrice { get; set; }
        public List<CustomPropertyModel> CustomProperty {  get; set; } 
        public string Description {  get; set; }
        public string Img { get; set; }
        public override string ToString()
        {
            string msg = string.Empty;
            msg += $"GenCompany:{GenCompany} ";
            msg += $"ProductGroup:{ProductGroup} ";
            msg += $"ProductType:{ProductType} ";
            msg += $"ProductName:{ProductName} ";
            msg += $"ProductOriginPrice:{ProductOriginPrice} ";
            msg += $"ProductSellPrice:{ProductSellPrice} ";
            #if true
            if (CustomProperty == null) return msg;
            foreach (var cProperty in CustomProperty)
            {
                msg += $"Key:{cProperty.Key} Value:{cProperty.Value}\n";
            }
            msg += "#######################################";
            #endif
            return msg;
        }

    }
}
