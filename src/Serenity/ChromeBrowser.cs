using FubuCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using StoryTeller;

namespace Serenity
{
    public class ChromeBrowser : BrowserLifecycle
    {
        public const string ChromeProcess = "chrome";
        public const string DriverProcess = "chromedriver";
        public const string File = "chromedriver.exe";

        public override string BrowserName { get { return "Chrome"; } }

        protected override IWebDriver buildDriver()
        {
            var fileSystem = new FileSystem();
            var settings = StoryTellerEnvironment.Get<SerenityEnvironment>();

            return fileSystem.FileExists(settings.WorkingDir, File)
                ? new ChromeDriver(settings.WorkingDir)
                : new ChromeDriver();
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(DriverProcess, File, ChromeProcess);
        }
    }
}