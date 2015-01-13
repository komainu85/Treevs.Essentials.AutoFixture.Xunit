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

        private const string AutoSetupExternalSourceFieldName = "AutoSetupSource";

        private readonly string[] _fixtureSetups;

        private readonly Type _classSource;

        private readonly IEnumerable<ISetupActionsProvider> _setupActionsProviders;

        public AutoSetupAttribute(params string[] fixtureSetups) :
            this(null, fixtureSetups)
        {
        }

        public AutoSetupAttribute(Type externalClassSource, params string[] fixtureSetups) :
            this(
               new List<ISetupActionsProvider>
                   {
                       new StaticMethodSetupActionsProvider(), 
                       new StaticPropertySetupActionsProvider()
                   },
               externalClassSource,
               fixtureSetups)
        {
        }

        public AutoSetupAttribute(
            IEnumerable<ISetupActionsProvider> setupActionsProviders,
            Type externalClassSource,
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

            _fixtureSetups = fixtureSetups;
            Fixture.Register(() => Fixture); // allows tests to request the fixture instance
            _setupActionsProviders = setupActionsProviders;
            _classSource = externalClassSource;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
        {
            var finalClassSourceType = 
                _classSource ??
                SetupActionsUtils.GetActionSourceType(methodUnderTest.ReflectedType, AutoSetupExternalSourceFieldName) ?? 
                methodUnderTest.ReflectedType;

            foreach (var action in this.GetSetups(finalClassSourceType))
            {
                action(Fixture);
            }

            return base.GetData(methodUnderTest, parameterTypes);
        }

        public IEnumerable<Action<IFixture>> GetSetups(Type functionSourceType)
        {
            var setupActions = new List<Action<IFixture>>();

            foreach (var fixtureSetup in _fixtureSetups.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var setups = _setupActionsProviders
                    .SelectMany(p => p.GetSetupActions(functionSourceType, fixtureSetup))
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
