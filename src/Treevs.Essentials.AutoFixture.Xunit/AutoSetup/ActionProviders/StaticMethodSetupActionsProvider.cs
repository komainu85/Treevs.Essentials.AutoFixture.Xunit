namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ploeh.AutoFixture;

    public class StaticMethodSetupActionsProvider : ISetupActionsProvider
    {
        public IEnumerable<Action<IFixture>> GetSetupActions(Type type, string fixtureAction)
        {
            var method = type.GetMethod(
               fixtureAction,
               BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (method == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = method.Invoke(null, new object[] { });

            try
            {
                return SetupActionsServices.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Method {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }
    }
}
