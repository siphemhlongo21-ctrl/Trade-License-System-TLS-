namespace C8.Console.Application
{
    public static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
           // var test = new Exchange.Services.EmailHelper().EmailTemplateAsString();
            var core = new NotificationCore();
            var task = new FunctionCore();

             core.GenerateDailyReminders();
             task.ProcessBackgroundTasks();

             

        }
    }
}
