using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace Clawfoot.TestUtilities
{
    public class XUnitSerializable : IXunitSerializable
    {
        public void Deserialize(IXunitSerializationInfo info)
        {
            Type thisType = this.GetType();
            PropertyInfo[] props = thisType
                .GetProperties()
                .Where(x => x.CanWrite)
                .ToArray();

            foreach (PropertyInfo prop in props)
            {
                Type propType = prop.PropertyType;
                if (propType.IsPrimitive || propType.IsEquivalentTo(typeof(string)))
                {
                    prop.SetValue(this, info.GetValue(prop.Name, propType));
                }
                else if (propType.IsValueType && propType.IsSerializable)
                {
                    prop.SetValue(this, info.GetValue(prop.Name, propType));
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(propType))
                {
                    prop.SetValue(this, new HashSet<string>(info.GetValue<string[]>(prop.Name)));
                }
                else
                {
                    //throw new NotSupportedException($"Property Type {propType.Name} is not supported.");
                }
            }
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            Type thisType = this.GetType();
            PropertyInfo[] props = thisType.GetProperties();

            foreach (PropertyInfo prop in props)
            {
                Type propType = prop.PropertyType;
                if (propType.IsPrimitive || propType.IsEquivalentTo(typeof(string)))
                {
                    info.AddValue(prop.Name, prop.GetValue(this));
                }
                else if (propType.IsValueType && propType.IsSerializable)
                {
                    info.AddValue(prop.Name, prop.GetValue(this));
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(propType))
                {
                    string[] testValue = ((IEnumerable<string>)prop.GetValue(this)).ToArray();
                    info.AddValue(prop.Name, ((IEnumerable<string>)prop.GetValue(this)).ToArray());
                }
                else
                {
                    //throw new NotSupportedException($"Property Type {propType.Name} is not supported.");
                }
            }
        }
    }
}
