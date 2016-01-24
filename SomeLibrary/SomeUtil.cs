using SomeLibrary.Logging;

namespace SomeLibrary
{
    public static class SomeUtil
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static void DoSomeThing()
        {
            using (LogProvider.OpenNestedContext("nested context"))
            {
                Log.Info("DoSomeThing");
            }
        }
    }
}