using System.Linq;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Xunit;

namespace LibLogTryOut.Tests
{
    public class Test
    {
        private readonly MemoryAppender _memoryAppender;

        public Test()
        {
            PatternLayout patternLayout = new PatternLayout
            {
                ConversionPattern = "[%-5level] %date [thread_id:%-3thread] %logger [%ndc] - %message%newline"
            };

            patternLayout.ActivateOptions();

            _memoryAppender = new MemoryAppender {Layout = patternLayout};
            
            BasicConfigurator.Configure(_memoryAppender);

            //default state of current logprovider
            SomeLibrary.Logging.LogProvider.SetCurrentLogProvider(null);
            SomeOtherLibrary.Logging.LogProvider.SetCurrentLogProvider(null);
        }

        [Fact]
        public void Default_LibLog__Should_Have_Log_Events()
        {
            //act
            SomeLibrary.SomeUtil.DoSomeThing();

            //assert
            Assert.True(
                _memoryAppender.GetEvents().Any(),
                "No log events.");
        }

        [Fact]
        public void Default_LibLog__Should_Have_Log_Event_With_NDC()
        {
            //arrange            
            const string ndcKey = "NDC";
            const string ndcValue = "nested context";

            //act
            SomeLibrary.SomeUtil.DoSomeThing();

            //assert
            Assert.True(
                _memoryAppender.GetEvents().Any(le => le.Properties.Contains(ndcKey)),
                "No log event with NDC present");

            Assert.True(
                _memoryAppender.GetEvents().Any(le => (string)le.Properties[ndcKey] == ndcValue),
                "Unexpected NDC value"
                );
        }

        [Fact]
        public void Default_LibLog__Should_Have_Log_Event_With_NDC_When_Setting_Current_LogProvider()
        {
            //arrange            
            const string ndcKey = "NDC";
            const string ndcValue = "nested context";

            //made internals visible to this test project
            var logProvider = SomeLibrary.Logging.LogProvider.ResolveLogProvider();

            SomeLibrary.Logging.LogProvider.SetCurrentLogProvider(logProvider);

            //act
            SomeLibrary.SomeUtil.DoSomeThing();

            //assert
            Assert.True(
                _memoryAppender.GetEvents().Any(le => le.Properties.Contains(ndcKey)),
                "No log event with NDC present");

            Assert.True(
                _memoryAppender.GetEvents().Any(le => (string)le.Properties[ndcKey] == ndcValue),
                "Unexpected NDC value"
                );
        }

        [Fact]
        public void Changed_LibLog__Should_Have_Log_Event_With_NDC()
        {
            //arrange            
            const string ndcKey = "NDC";
            const string ndcValue = "nested context";

            //act

            //this library uses a altered liblog version where the static methods OpenNestedContext 
            //and OpenMappedContext are changed. When there is no CurrentLogProvider, one will be
            //resolved.
            SomeOtherLibrary.SomeOtherUtil.DoSomeOtherThing();

            //assert
            Assert.True(
                _memoryAppender.GetEvents().Any(le => le.Properties.Contains(ndcKey)),
                "No log event with NDC present");

            Assert.True(
                _memoryAppender.GetEvents().Any(le => (string) le.Properties[ndcKey] == ndcValue),
                "Unexpected NDC value"
                );
        }
    }
}
