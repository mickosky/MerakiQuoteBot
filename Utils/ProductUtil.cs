using System;
using System.Collections.Generic;
using Jurumani.BotBuilder.Models;
using static Jurumani.BotBuilder.Models.AWSProductModel;

namespace  Jurumani.BotBuilder.Utils
{
    public static class ProductUtil
    {
        public static string SUFFIX = "-HW";
        public static string generateSKU(string model)
        {
            string result = string.Empty;
            int idx = model.IndexOf(SUFFIX);
            if (idx >= 0)
            {
                result = model.Remove(idx, SUFFIX.Length);
            }
            if (model.IndexOf("MX68CW") >= 0)
            {
                result = "MX68CW";
            }
            return result;

        }
        public static List<listJurumaniCloudInventory_ModelsContent.ItemContent> FilterforHardware(AWSProductModel productlist)
        {
            var word = "HW";
            var products = new List<listJurumaniCloudInventory_ModelsContent.ItemContent>() { };
            productlist.listJurumaniCloudInventory_Models.Items.ForEach(product =>
            {
                if (product.SKU.Contains("HW"))
                {
                    products.Add(product);
                }
            });
            return products;

        }
    }
    
}
