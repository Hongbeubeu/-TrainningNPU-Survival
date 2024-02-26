using System;
using System.Linq;
using Npu.Helper;

namespace Npu.Formula
{
    public static class ParameterUtils
    {
        public static string[] AttributedPropertyNames(this Type type)
        {
            return type.GetInstanceAttributedProperties<InnerParameterAttribute>()
                .Select(i => i.property.Name).ToArray();
        }

        public static IParameter GetAttributedProperty(this object target, string name)
        {
            var prop = target.GetType().GetInstanceAttributedProperties<InnerParameterAttribute>()
                .First(i => i.property.Name.Equals(name)).property;

            if (prop == null)
            {
                Logger.Error("ParameterUtils", $"Cannot find property {name} in {target}");
                return null;
            }

            return prop.GetValue(target, new object[0]) as IParameter;
        }
    }
}