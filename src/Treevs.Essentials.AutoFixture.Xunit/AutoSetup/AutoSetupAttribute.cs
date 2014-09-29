namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Xunit;

    public class AutoSetupAttribute : AutoDataAttribute
    {
        private const string DefaultFixtureSetupName = "AutoSetup";

        public string[] FixtureSetups { get; set; }

        private readonly IEnumerable<ISetupActionsProvider> _setupActionsProviders;

        public AutoSetupAttribute(params string[] fixtureSetups) :
            this(
               new List<ISetupActionsProvider>
                   {
                       new StaticFieldSetupActionsProvider(), 
                       new StaticMethodSetupActionsProvider(), 
                       new StaticPropertySetupActionsProvider()
                   },
               fixtureSetups)
        {
        }

        public AutoSetupAttribute(
            IEnumerable<ISetupActionsProvider> setupActionsProviders,
            params string[] fixtureSetups)
            : base(new Fixture())
        {
            if (!fixtureSetups.Any())
            {
                fixtureSetups = new[] { DefaultFixtureSetupName };
            }

            if (!fixtureSetups.Contains(DefaultFixtureSetupName))
            {
                fixtureSetups = new[] { DefaultFixtureSetupName }.Concat(fixtureSetups).ToArray();
            }

            this.FixtureSetups = fixtureSetups;
            this.Fixture.Register(() => this.Fixture);
            this._setupActionsProviders = setupActionsProviders;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            foreach (var action in this.GetSetups(methodUnderTest))
            {
                action(this.Fixture);
            }

            return base.GetData(methodUnderTest, parameterTypes);
        }

        public IEnumerable<Action<IFixture>> GetSetups(MethodInfo method)
        {
            var setupActions = new List<Action<IFixture>>();
            var type = method.DeclaringType;

            foreach (var fixtureSetup in this.FixtureSetups.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var setups = this._setupActionsProviders
                    .SelectMany(p => p.GetSetupActions(type, fixtureSetup))
                    .ToList();

                if (!setups.Any() && !fixtureSetup.Equals(DefaultFixtureSetupName, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentOutOfRangeException(fixtureSetup, "No static property, method or field could be found on the test fixture with the name " + fixtureSetup);
                }

                setupActions.AddRange(setups);
            }

            return setupActions;
        }
    }
}
