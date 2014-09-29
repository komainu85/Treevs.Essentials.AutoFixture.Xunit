namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ploeh.AutoFixture;

    public class StaticFieldSetupActionsProvider : ISetupActionsProvider
    {
        public IEnumerable<Action<IFixture>> GetSetupActions(Type type, string fixtureAction)
        {
            var field = type.GetField(
                 fixtureAction,
                 BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var obj = field.GetValue(null);

            try
            {
                return SetupActionsUtils.ParseFixtureActionValue(obj);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Field {0} on {1} did not return IEnumerableAction<IFixture>> or Action<IFixture>",
                        fixtureAction,
                        type.FullName));
            }
        }
    }
}
