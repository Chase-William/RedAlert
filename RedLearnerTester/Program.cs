using RedLearner;
using System;
using System.Threading.Tasks;

namespace RedAlertBotTester
{
    class Program
    {        
        static void Main(string[] args)
        {
            RedAlertBot.RedAlertBot.Bot.Init(IntPtr.Zero);
            RedAlertBot.RedAlertBot.Bot.IsBotEnabled = true;
            Task.Delay(-1).Wait();
        }
    }
}
