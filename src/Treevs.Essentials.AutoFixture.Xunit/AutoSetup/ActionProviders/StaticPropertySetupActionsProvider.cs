namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup.ActionProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ploeh.AutoFixture;

    public class StaticPropertySetupActionsProvider : ISetupActionsProvider
    {
        public IEnumerable<Action<IFixture>> GetSetupActions(Type type, string fixtureAction)
        {
            var property = type.GetProperty(
                fixtureAction,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (property == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = property.GetValue(null);
            try
            {
                return SetupActionsServices.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Property {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }
    }
}
