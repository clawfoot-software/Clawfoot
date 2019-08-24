using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Clawfoot.TestUtilities
{
    public class TestModelBuilder<T>
    {
        private List<Action> actions = new List<Action>();
        private T instance;

        public TestModelBuilder()
        {
            instance = CreateInstance();
        }

        public TestModelBuilder(T instance)
        {
            this.instance = instance;
        }

        //---------------------------------------------------------
        //===== Public Methods =====


        public T CreateInstance()
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        }

        public TestModelBuilder<T> WithValue(string memberName, object value, MemberTypes memberType = MemberTypes.Property)
        {
            actions.Add(() => SetValue(memberName, value, memberType));
            return this;
        }

        public T Build()
        {
            foreach (Action action in actions)
            {
                action.Invoke();
            }

            return instance;
        }


        //---------------------------------------------------------
        //===== Private Helpers =====

        private void SetValue(string name, object value, MemberTypes memberType)
        {
            if (memberType == MemberTypes.Property)
            {
                SetPropertyValue(name, value);
            }
            else if (memberType == MemberTypes.Field)
            {
                SetFieldValue(name, value);
            }
        }

        private void SetPropertyValue(string name, object value)
        {
            Type type = typeof(T);
            GetProperty(type, name).SetValue(instance, value);
        }

        private void SetFieldValue(string name, object value)
        {
            Type type = typeof(T);
            GetField(type, name).SetValue(instance, value);
        }

        private PropertyInfo GetProperty(Type type, string name)
        {
            PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new System.InvalidOperationException($"Cannot create type: Type \"{type.Name}\" does not contain a property named \"{name}\" ");
            }
            return property;
        }

        private FieldInfo GetField(Type type, string name)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new System.InvalidOperationException($"Cannot create type: Type \"{type.Name}\" does not contain a field named \"{name}\" ");
            }
            return field;
        }
    }
}
