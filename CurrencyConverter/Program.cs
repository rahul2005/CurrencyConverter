namespace CurrencyConverter
{
    /// <summary>
    /// Main entry <see cref="Program"/>
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry <see cref="Main(string[])"/>
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// create host <see cref="CreateHostBuilder(string[])"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
