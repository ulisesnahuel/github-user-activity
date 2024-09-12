
using Github_Activity;

class Program
{
    static async Task Main(string[] args)
    {
        
        

        Console.Write("github-activity ");
        string userName = Console.ReadLine();

        UserActivity userActivity = new UserActivity(userName);

        await userActivity.GetData();

    }
}