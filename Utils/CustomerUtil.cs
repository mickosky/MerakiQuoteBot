using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jurumani.BotBuilder.Models;
using Jurumani.BotBuilder.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jurumani.BotBuilder.Utils
{
    public  static class CustomerUtil
    {
      
        private static string endpoint = "crm.contact.list";
      
        public static  List<CustomerModel> getCustomerByPhoneNumber(string Phone)
        {
            string jsonData = @"{
              'order': {
                'DATE_CREATE': 'ASC'
              },
              'filter': {
                'PHONE':'0"+Phone.ToString()+@"'
              },
	            'select': [
                'ID',
                'NAME',
                'TYPE_ID',
                'LAST_NAME',
                'PHONE',
                'EMAIL',
                'COMPANY_ID'
              ]
                }";
            JObject body = JObject.Parse(jsonData);
            var result= WebServicesFactory.FetchData(body.ToString(),endpoint);
        
            var jsonresult = result.Content.ReadAsStringAsync();

            List<CustomerModel> customers = new List<CustomerModel>() { };
            foreach (var item in (JToken.Parse(jsonresult.Result)["result"]))
            {
               var customer = item.ToString();
                customers.Add(JsonConvert.DeserializeObject<CustomerModel>(customer));
            }
            return customers;
        }
        public static  string saveCustomerInformation(string name,string surname,string companyid,string email,string phonenumber)
        {
            string bodystring = @"
                        {'fields': {
                          'NAME':'" +name+ @"',
                          'LAST_NAME':'" +surname+ @"',
                          'OPENED':'Y',
                          'ASSIGNED_BY_ID': '13',
                          'TYPE_ID':'CLIENT',
                          'SOURCE_ID':'CHATBOT',
                          'COMPANY_ID':'"+companyid+@"',
                          'EMAIL':'"+email+@"',
                          'PHONE':[
                            {
                              'VALUE':'0" + phonenumber + @"',
                              'VALUE_TYPE':'WORK'
                            }
                          ]
                            }               
                        }
                    ";
            JObject body = JObject.Parse(bodystring);
            var response = WebServicesFactory.FetchData(body.ToString(), "crm.contact.add");
            var jsonresult = response.Content.ReadAsStringAsync();
            //extract
            var contactId = JToken.Parse(jsonresult.Result)["result"].ToString();
           
            return contactId;
        }
       
    }
}
