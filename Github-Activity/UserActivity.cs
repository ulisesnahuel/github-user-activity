using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Github_Activity
{
    public class UserActivity
    {
        private string UserName { get; set; }
        public UserActivity(string userName)
        {
            UserName = userName;
        }

        public async Task GetData()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://api.github.com/users/{UserName}/events";
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");

                try
                {

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        var events = JsonDocument.Parse(data).RootElement;


                        Dictionary<string, int> commitsByRepo = new Dictionary<string, int>();

                        foreach (var eventItem in events.EnumerateArray())
                        {
                            string type = eventItem.GetProperty("type").GetString();
                            string repoName = eventItem.GetProperty("repo").GetProperty("name").GetString();

                            if (type == "PushEvent")
                            {
                                var commitCount = eventItem.GetProperty("payload").GetProperty("commits").GetArrayLength();

                                if (commitsByRepo.ContainsKey(repoName))
                                {
                                    commitsByRepo[repoName] += commitCount;
                                }
                                else
                                {
                                    commitsByRepo[repoName] = commitCount;
                                }
                            }                          
                        }
                        foreach (var repo in commitsByRepo)
                        {
                            Console.WriteLine($"- Pushed {repo.Value} commits to {repo.Key}");
                        }

                        foreach (var eventItem in events.EnumerateArray())
                        {
                            string type = eventItem.GetProperty("type").GetString();
                            string repoName = eventItem.GetProperty("repo").GetProperty("name").GetString();

                            switch (type)
                            {
                                //case "PushEvent":
                                //    var commits = eventItem.GetProperty("payload").GetProperty("commits").GetArrayLength();
                                //    Console.WriteLine($"- Pushed {commits} commits to {repoName}");
                                //    break;
                                case "IssuesEvent":
                                    string action = eventItem.GetProperty("payload").GetProperty("action").GetString();
                                    Console.WriteLine($"- {action} a new issue in {repoName}");
                                    break;
                                case "WatchEvent":
                                    Console.WriteLine($"- Starred {repoName}");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Excepción: {ex.Message}");
                }


            }
        }
    }
}

