using System;
using System.Collections.Generic;

namespace Jurumani.BotBuilder.Models
{
    public class CustomerModel
    {
        public string NAME {get;set;}
        public string LAST_NAME { get; set; }
        public List<Generics> PHONE { get; set; }
        public List<Generics> EMAIL { get; set; }
        public string ID { get; set; }
        public string COMPANY_ID { get; set; }
        public CustomerModel()
        {
        }
        public CustomerModel(string Id,String  paramName,string paramSurname,List<Generics> paramPhone, List<Generics> paramEmail)
        {
            NAME = paramName;
            LAST_NAME = paramSurname;
            PHONE= paramPhone;
            EMAIL = paramEmail;
            ID = Id;

        }
        public override string ToString()
        {
            return $"Name: {NAME} \n" +
                $"lastname:{LAST_NAME} \n" +
                $"phone:0{PHONE[0].VALUE}\n" +
                $"email:{EMAIL[0].VALUE}\n";
                
        }

    }
    public  class Generics
    {
        public string ID { get; set; }
        public string VALUE_TYPE { get; set; }
        public string VALUE { get; set; }
        public string TYPE_ID { get; set; }
    }
}
