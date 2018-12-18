using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ValveController.Models
{
    class Repository
    {
        public async Task<List<Users>> GetUsers()
        {
            List<Users> services;
            var URLwebAPI = "http://valvecontroller.azurewebsites.net/tables/users?zumo-api-version=2.0.0";
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(URLwebAPI);
                services = JsonConvert.DeserializeObject<List<Users>>(JSON);
            }
            return services;
        }
    }
}