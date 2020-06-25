using System;
using Jurumani.BotBuilder.Utils;
using Newtonsoft.Json.Linq;
using Jurumani.BotBuilder.Models;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Jurumani.BotBuilder.Utils
{
    public static class DealsUtil
    {
        public static string DEALENDPOINT = "crm.deal.add";
        public static string QUOTEENDPOINT = "crm.quote.add";
        public static string CreateDeal(string paramtitle,string companyID,string contactid,string endpoint)
        {
           
            string bodystring = @"
                {
                  'fields':{
                    'TITLE':'"+paramtitle+@"',
                    'TYPE_ID':'SD_WAN',
                    'COMPANY_ID':'" +companyID+ @"',
                    'CONTACT_ID':'"+contactid+@"',
                    'OPENED':'Y',
                    'ASSIGNED_BY_ID':'13',
                    'ORIGINATOR_ID':'QUOTE_CHAT_BOT',
                    'CATEGORY_ID':'2'


                  }

                    }";

            JObject body = JObject.Parse(bodystring);
            var result = WebServicesFactory.FetchData(body.ToString(), endpoint);

            var jsonresult = result.Content.ReadAsStringAsync();
        
            var dealID = JToken.Parse(jsonresult.Result)["result"].ToString();

            return dealID ;
                    
        }
        public static string CreateQuote(string customername, string customersurname, string companyname,string dealid,string customeremail,string expirydate,string terms, string endpoint)
        {

            string bodystring = @"
                {
        'fields':
        {
                'TITLE':'Quote for "+customername +" "+customersurname+"-"+companyname +@"', 
            'TYPE_ID':'SDWAN',
            'STAGE_ID':'NEW',
            'DEAL_ID':'"+dealid+@"',
            'CLIENT_EMAIL':'"+customeremail+@"',
            'CURRENCY_ID':'ZAR',
            'CATEGORY_ID':'2',
            'CLOSEDATE':'"+expirydate+@"',
            'TERMS':'"+terms+@"'
        }
        }";

 
            JObject body = JObject.Parse(bodystring);
       
            var result = WebServicesFactory.FetchData(body.ToString(), endpoint);

            var jsonresult = result.Content.ReadAsStringAsync();
            var status = JToken.Parse(jsonresult.Result)["result"].ToString();
            return status;

        }
        public static string AddProductsToQuote(string quoteid, List<BitrixProductRowModel> productlist,string endpoint)
        {
            
           
            
            string requestbody = @"
               
           {
            'id': '" + quoteid + @"', 
            'rows':" + JsonConvert.SerializeObject(productlist) + @"
             }";

            JObject body = JObject.Parse(requestbody);
        
            var result = WebServicesFactory.FetchData(body.ToString(), endpoint);

            var jsonresult = result.Content.ReadAsStringAsync();
            return JToken.Parse(jsonresult.Result)["result"].ToString();


        }
    }
}
