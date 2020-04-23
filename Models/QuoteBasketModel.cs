using System;
using System.Collections.Generic;

namespace Jurumani.BotBuilder.Models
{
    public class QuoteBasketModel
    {
        public CustomerModel customer{ get; set; }
        public string quoteID { get; set; }
        public string dealID { get; set; }
        public string licenseDuration { get; set; }
        public  List<BitrixProductRowModel> products { get; set; }
        public override string ToString()
        {
            return $"You now have {products.Count} line items in your quote";
          
       }
        public  QuoteBasketModel()
        {
            products = new List<BitrixProductRowModel> { };
            customer = new CustomerModel();
        }

        public void clearProducts()
        {
            products.Clear();
        }
        public void updateCustomer(CustomerModel cust)
        {
            customer = cust;
        }
        public void updatequoteID(string quoteid)
        {
            quoteID = quoteid;
        }
        
    }
   
}
