namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;

    using Ploeh.AutoFixture;

    public interface ISetupActionsProvider
    {
        IEnumerable<Action<IFixture>> GetSetupActions(Type type, string fixtureAction);
    }
}
