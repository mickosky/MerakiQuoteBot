using System;
namespace Jurumani.BotBuilder.Models

{
    public class CompanyModel
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string INDUSTRY { get; set; }
        public string ASSIGNED_BY_ID { get; set; }
        public string COMPANY_TYPE { get; set; }
        
        public CompanyModel(int id, string name,string industry,string assignee)
        {
            ID = id;
            NAME = name;
            INDUSTRY = industry;
            ASSIGNED_BY_ID = assignee;


        }
    }
}
