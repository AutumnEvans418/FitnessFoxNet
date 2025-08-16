namespace FitnessFox.Components.Services
{
    public class LoggingService : ILoggingService
    {
        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
