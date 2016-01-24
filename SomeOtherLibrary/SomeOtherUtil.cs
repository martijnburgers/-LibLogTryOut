using SomeOtherLibrary.Logging;

namespace SomeOtherLibrary
{
    public static class SomeOtherUtil
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static void DoSomeOtherThing()
        {
            using (LogProvider.OpenNestedContext("nested context"))
            {
                Log.Info("DoSomeOtherThing");
            }
        }
    }
}
