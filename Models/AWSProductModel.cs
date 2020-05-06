using System;
using System.Collections.Generic;

namespace Jurumani.BotBuilder.Models
{
    public class AWSProductModel
    {
        public listJurumaniCloudInventory_ModelsContent listJurumaniCloudInventory_Models { get; set; }

        public class listJurumaniCloudInventory_ModelsContent
        {
            public List<ItemContent> Items { get; set; }
            public class ItemContent
            {
                public string SKU { get; set; }
                public string LIST_PRICE_USD { get; set; }
            }
        }
    }
}
