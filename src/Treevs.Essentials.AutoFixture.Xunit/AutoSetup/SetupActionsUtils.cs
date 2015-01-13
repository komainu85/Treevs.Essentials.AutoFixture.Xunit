namespace Treevs.Essentials.AutoFixture.Xunit.AutoSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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

        public static Type GetActionSourceTypeField(Type type, string fieldName)
        {
            var member = type.GetField(
                fieldName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (member == null)
            {
                return null;
            }

            var sourceType = member.GetValue(null) as Type;

            if (sourceType == null)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Field {0} on {1} did not return a Type value",
                        fieldName,
                        type.FullName));
            }

            return sourceType;
        }

        public static Type GetActionSourceTypeProperty(Type type, string fieldName)
        {
            var member = type.GetProperty(
                fieldName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (member == null)
            {
                return null;
            }

            var sourceType = member.GetValue(null) as Type;

            if (sourceType == null)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Property {0} on {1} did not return a Type value",
                        fieldName,
                        type.FullName));
            }

            return sourceType;
        } 
    }
}
