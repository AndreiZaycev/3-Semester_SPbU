using System;
using System.Threading.Tasks;

namespace Test
{
    public class Chat
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 1)
            {
                var server = new Server(int.Parse(args[0]));
                await server.Run();
            }
            else if (args.Length == 2)
            {
                var client = new Client(args[0], int.Parse(args[1]));
                await client.Run();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(args));
            }
        }
    }
}
