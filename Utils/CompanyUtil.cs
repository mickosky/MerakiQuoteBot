using System;
using Jurumani.BotBuilder.Models;
using Jurumani.BotBuilder.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jurumani.BotBuilder.Utils
{
    public static class CompanyUtil
    {
       public static string SaveCompanyDetails(string Companytitle) { 
       
            //put together the body of the request
            var request = @"
                   {'fields': {
                        'TITLE':'"+Companytitle+@"',
                        'COMPANY_TYPE':'CUSTOMER',
                        'OPENED': 'Y',
                        'ASSIGNED_BY_ID': '13'
                      }
                        }";
            //send to bitrix
            JObject body = JObject.Parse(request);
            var response = WebServicesFactory.FetchData(body.ToString(), "crm.company.add");
            var jsonresult = response.Content.ReadAsStringAsync();
            //extract
            var companyID =JToken.Parse(jsonresult.Result)["result"].ToString();
            //return JToken.Parse(jsonresult.Result).ToString();
            //return body.ToString();

            return companyID;
        }
        public static string GetCompanyById(string CompanyID)
        {
            //put together the body of the request
            var request = @"
                    {
	                    'ID':'"+ Int32.Parse(CompanyID)+@"'
                     }";
            //send to bitrix
            JObject body = JObject.Parse(request);
           
            var response = WebServicesFactory.FetchData(body.ToString(), "crm.company.get");
            var jsonresult = response.Content.ReadAsStringAsync();

            var companydata = JToken.Parse(jsonresult.Result);
            return companydata["result"]["TITLE"].ToString();

            //extract
            //var companyObject = JsonConvert.DeserializeObject<CompanyModel>(companydata.ToString());
            //return companyObject;
        }
    }
}
