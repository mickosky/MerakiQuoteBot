using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Jurumani.BotBuilder.Models.AWSProductModel;

namespace  Jurumani.BotBuilder.Utils
{

    public static class ProductUtil
    {
        private static string CURRENCY_CONVERTER_URL = "https://api.exchangeratesapi.io/latest?base=USD&symbols=ZAR";
        public static string SUFFIX = "-HW";
        static HttpClient httpClient = new HttpClient();
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
        public static async Task<double> ConvertProductPrice()
        {
            double zar_price = 0;

            HttpResponseMessage response = await httpClient.GetAsync(CURRENCY_CONVERTER_URL);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseBody);
            zar_price = (double)jsonObject["rates"]["ZAR"];


            return Math.Round(zar_price,2);

        }


    }
}
