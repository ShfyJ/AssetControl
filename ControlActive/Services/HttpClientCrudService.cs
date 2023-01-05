using ControlActive.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;



namespace ControlActive.Services
{
    public class HttpClientCrudService : IHttpClientServiceImplementation
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        private readonly JsonSerializerOptions _options;
        public HttpClientCrudService()
        {
            //_httpClient.BaseAddress = new Uri("");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task Execute(string cad_num)
        {
            await CreateCompanyWithHttpRequestMessage(cad_num);
        }

		private async Task CreateCompanyWithHttpRequestMessage(string cad_num)
		{
            Random random = new Random();
			var companyForCreation = new CompanyForCreationDto
			{
				id = random.Next(),
				cad_num = cad_num,
				time = DateTime.Now
			};

            
            Uri myUri = new ("http://10.190.6.106:8080/api/reestr");

            var authString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("uzbekneftegaz:uzbekneftegaz@87218964741"));
            var company = JsonSerializer.Serialize(companyForCreation);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
            //var requestContent = new StringContent(company, Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsync("http://10.190.6.106:8080/api/", requestContent);

            var request = new HttpRequestMessage(HttpMethod.Post, myUri);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            request.Content = new StringContent(company, Encoding.UTF8);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
             var response = await _httpClient.SendAsync(request);
             response.EnsureSuccessStatusCode();

             var content = await response.Content.ReadAsStringAsync();
             var createdCompany = JsonSerializer.Deserialize<CompanyResponseDto>(content, _options);

              
            }

            catch (Exception ex)
            {
                ex.ToString();
            }


            

        }
	}
}


//var webAdd = "http://10.190.6.106:8080/api/reestr";
//var httpWebReq = (HttpWebRequest)WebRequest.Create(webAdd);
//httpWebReq.ContentType = "application/json";
//httpWebReq.Method = "POST";
//string result;
//var credBuffer = new UTF8Encoding().GetBytes("uzbekneftegaz:uzbekneftegaz@87218964741");
//httpWebReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credBuffer);
//// httpWebReq.Headers["Authorization"] = "Basic";
//// httpWebReq.Headers["password"] = BCrypt.Net.BCrypt.HashPassword(TextToConnect);

//string data = "id=123&cad_num=" + cad_num + "&time=" + DateTime.Now; //replace <value>
//byte[] dataStream = Encoding.UTF8.GetBytes(data);


//using (WebResponse response = await httpWebReq.GetResponseAsync())
//{

//    var d = response;

//    using (var streamReader = new StreamReader(response.GetResponseStream()))
//    {

//        result = streamReader.ReadToEnd();


//    }
//}