using Newtonsoft.Json;
using RareCrewTask.Models;

namespace RareCrewTask.Services
{
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiUrl;
        private readonly string ApiKey;

        public EmployeeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            ApiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            ApiKey = configuration.GetValue<string>("AppSettings:ApiKey");
        }


        public async Task<List<EmployeeTimeTrackModel>> GetEmployeeTimeTracksAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{ApiUrl}/gettimeentries?code={ApiKey}");

                var data = JsonConvert.DeserializeObject<List<EmployeeTimeTrackModel>>(response);

                return data ?? new List<EmployeeTimeTrackModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
