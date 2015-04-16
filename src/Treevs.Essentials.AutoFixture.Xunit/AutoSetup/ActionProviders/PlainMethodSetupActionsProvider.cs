using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup.ActionProviders
{
    public class PlainMethodSetupActionsProvider : ISetupActionsProvider
    {
        public IEnumerable<Action<IFixture>> GetSetupActions(Type type, string fixtureAction)
        {
            var method = type.GetMethod(
               fixtureAction,
               BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (method == null ||
                method.ReturnType != typeof(void))
            {
                return Enumerable.Empty<Action<IFixture>>();
            }

            try
            {
                Action<IFixture> action = (IFixture fixture) =>
                {
                    var methodParams = method.GetParameters();
                    var values = new object[methodParams.Length];

                    for (int i = 0; i < methodParams.Length; i++)
                    {
                        var ctx = new SpecimenContext(fixture);
                        values[i] = ctx.Resolve(new SeededRequest(methodParams[i].ParameterType, null));
                    }

                    method.Invoke(null, values);
                };

                return new [] { action };
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
