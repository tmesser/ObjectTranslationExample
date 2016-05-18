using System;
using System.Linq;

namespace ObjectTranslationExample
{
    public static class TypeSwitch
    {
        public static void Do(object firstSource, object secondSource, params DualCaseInfo[] cases)
        {
            var firstType = firstSource.GetType();
            var secondType = secondSource.GetType();
            Do(firstType, secondType, cases);
        }

        public static void Do(Type firstSource, Type secondSource, params DualCaseInfo[] cases)
        {
            foreach (var entry in cases.Where(entry =>
                (entry.IsDefault || entry.FirstTarget.IsAssignableFrom(firstSource)) &&
                (entry.IsDefault || entry.SecondTarget.IsAssignableFrom(secondSource))))
            {
                entry.Action(firstSource, secondSource);
                break;
            }
        }

        public static DualCaseInfo Case<T, TS>(Action action)
        {
            return new DualCaseInfo
            {
                Action = (x, y) => action(),
                FirstTarget = typeof(T),
                SecondTarget = typeof(TS)
            };
        }

        public static DualCaseInfo Case<T, TS>(Action<T, TS> action)
        {
            return new DualCaseInfo
            {
                Action = (x, y) => action((T)x, (TS)y),
                FirstTarget = typeof(T),
                SecondTarget = typeof(TS)
            };
        }
    }
}
