using FubuCore;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller;
using StoryTeller.Domain;
using StoryTeller.Engine;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing.Fixtures
{
    [TestFixture(typeof(FirefoxBrowser))]
    [TestFixture(typeof(ChromeBrowser))]
    // TODO: Uncomment when we setup the IE Browser
    // [TestFixture(typeof(InternetExplorerBrowser))]
    [TestFixture(typeof(PhantomBrowser))]
    public abstract class ScreenManipulationTester<TBrowser> : ScreenFixture where TBrowser : IBrowserLifecycle, new()
    {
        private IBrowserLifecycle _browserLifecycle;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            BrowserForTesting.Use<TBrowser>();
            theDriver = BrowserForTesting.Driver;
        }

        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteFile("screenfixture.htm");

            var document = new HtmlDocument();
            configureDocument(document);

            document.WriteToFile("screenfixture.htm");

            theDriver.Navigate().GoToUrl("file:///" + "screenfixture.htm".ToFullPath());
            theFixture = new StubScreenFixture(theDriver);
        }

        protected StubScreenFixture theFixture;
        protected IWebDriver theDriver { get; private set; }

        protected abstract void configureDocument(HtmlDocument document);
    }


    public class StubScreenFixture : ScreenFixture
    {
        public StubScreenFixture(IWebDriver driver)
        {
            PushElementContext(driver);
        }
    }

    public class StepExecutionResult
    {
        public IStepResults Results { get; set; }
        public Counts Counts { get; set; }
    }

    public static class TestingExtensions
    {
        public static StepExecutionResult Execute(this IGrammar grammar, IStep step)
        {
            var context = new TestContext();

            grammar.Execute(step, context);

            return new StepExecutionResult{
                Counts = context.Counts,
                Results = context.ResultsFor(step)
            };
        }

        public static StepExecutionResult Execute(this IGrammar grammar)
        {
            return grammar.Execute(new Step());
        }

        public static void ShouldEqual(this Counts counts, int rights, int wrongs, int exceptions, int syntaxErrors)
        {
            var expected = new Counts
                           {
                               Rights = rights,
                               Wrongs = wrongs,
                               Exceptions = exceptions,
                               SyntaxErrors = syntaxErrors
                           };

            Assert.AreEqual(expected, counts);
        }
    }
}