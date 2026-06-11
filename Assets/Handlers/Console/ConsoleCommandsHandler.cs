using Handlers.Console;

namespace Assets.Handlers.Console
{
    public class ConsoleCommandsHandler
    {
        private static ConsoleCommandsHandler instance;
        public static ConsoleCommandsHandler Instance
        {
            get
            {
                if (instance == null) instance = new ConsoleCommandsHandler();
                return instance;
            }
        }

        public ConsoleCommandsHandler() { }
    }
}
