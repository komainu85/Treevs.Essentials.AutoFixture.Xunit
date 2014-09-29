namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ploeh.AutoFixture;

    public class SetupActionsUtils
    {
        public static IEnumerable<Action<IFixture>> ParseFixtureActionValue(object obj)
        {
            if (obj == null)
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            var enumerable = obj as IEnumerable<Action<IFixture>>;
            if (enumerable == null)
            {
                var single = obj as Action<IFixture>;
                if (single == null)
                {
                    throw new ArgumentNullException();
                }
                enumerable = new List<Action<IFixture>> { single };
            }

            return enumerable;
        }
    }
}
