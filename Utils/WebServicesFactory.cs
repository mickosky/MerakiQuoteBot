using System;
using System.Linq;

using System.Text;
using GraphQL.Client.Http;
using System.Net.Http;
using GraphQL.Client.Serializer.Newtonsoft;

using Jurumani.BotBuilder.Models;
using GraphQL;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Jurumani.BotBuilder.Utils
{
    public static class WebServicesFactory {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _remoteServiceBaseUrl = "https://hive.jurumani.com/rest/13/tmsxut13t2xryuej/";
      

        public static HttpResponseMessage FetchData(string requestDTO, string endpoint)
        {
            //convert the requestDto into json
            //var body= JsonConvert.SerializeObject(requestDTO);
            var requestData = new StringContent(requestDTO, Encoding.UTF8, "application/json");
            //send request to get the data
            var response = _httpClient.PostAsync(_remoteServiceBaseUrl + endpoint, requestData);

            //parse the data into the response DTO of the type

            return response.Result;

        }
        public  static async Task<AWSProductModel> QueryProductData(string paramcontains, string paramnotcontains)
        {
            using var graphQLClient = new GraphQLHttpClient("https://7ybj32hnhnaafezpkkrjfm462i.appsync-api.us-east-1.amazonaws.com/graphql", new NewtonsoftJsonSerializer());
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("x-api-key", "da2-gmltq7ghjneazfyal4m3svz5s4");
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Context-Type", "application/json");

            var ProductRequest = new GraphQLRequest
            {
                Query = @"query filter1($contains: String!, $notcontains: String!) {
  listJurumaniCloudInventory_Models(limit:1000,filter: {SKU: {contains: $contains, notContains: $notcontains}}) {
    items {
      SKU
      LIST_PRICE_USD
    }
  }
}"
,
                OperationName = "filter1",
                Variables = new
                {
                    contains = paramcontains,
                    notcontains = paramnotcontains
                }
            };

            try
            {
                var graphQLResponse = await graphQLClient.SendQueryAsync<AWSProductModel>(ProductRequest);
                return graphQLResponse.Data;

            }
            catch (Exception ex)
            {
                return new AWSProductModel() ;
            }
        }

    }


}

