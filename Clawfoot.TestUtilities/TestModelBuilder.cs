using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Clawfoot.TestUtilities
{
    public static class TestModelBuilder
    {
        public static TestModelBuilder<TModel> For<TModel>()
            where TModel : class
        {
            return new TestModelBuilder<TModel>();
        }
    }
    
    public class TestModelBuilder<TModel> where TModel : class
    {
        private List<Action> actions = new List<Action>();
        private TModel instance;
        
        public TestModelBuilder()
        {
            instance = CreateInstance();
        }

        public TestModelBuilder(TModel instance)
        {
            if(instance is null) throw new ArgumentNullException(nameof(instance), "Cannot create a TestModelBuilder with a null instance through this constructor");
            this.instance = instance;
        }

        //---------------------------------------------------------
        //===== Public Methods =====

        /// <summary>
        /// Creates a new, empty, instance of TModel
        /// </summary>
        /// <returns></returns>
        public TModel CreateInstance()
        {
            return (TModel)Activator.CreateInstance(typeof(TModel), true);
        }

        public TestModelBuilder<TModel> With<TMember>(Expression<Func<TModel, TMember>> memberExpression, TMember value, MemberTypes memberType = MemberTypes.Property)
        {
            string memberName = ((MemberExpression)memberExpression.Body).Member.Name;
            actions.Add(() => Set(memberName, value, memberType));
            return this;
        }
        
        public TestModelBuilder<TModel> With<TMember>(Expression<Func<TModel, TMember>> memberExpression, Func<TestModelBuilder<TMember>, TestModelBuilder<TMember>> builder, MemberTypes memberType = MemberTypes.Property)
            where TMember : class
        {
            TMember value = builder(new TestModelBuilder<TMember>()).Build();
            string memberName = ((MemberExpression)memberExpression.Body).Member.Name;
            actions.Add(() => Set(memberName, value, memberType));
            return this;
        }

        public TestModelBuilder<TModel> With(string memberName, object value, MemberTypes memberType = MemberTypes.Property)
        {
            actions.Add(() => Set(memberName, value, memberType));
            return this;
        }

        /// <summary>
        /// Builds the model
        /// </summary>
        /// <returns></returns>
        public TModel Build()
        {
            foreach (Action action in actions)
            {
                action.Invoke();
            }

            return instance;
        }


        //---------------------------------------------------------
        //===== Private Helpers =====

        private void Set(string name, object value, MemberTypes memberType)
        {
            if (memberType == MemberTypes.Property)
            {
                SetProperty(name, value);
            }
            else if (memberType == MemberTypes.Field)
            {
                SetField(name, value);
            }
        }

        private void SetProperty(string name, object value)
        {
            Type type = typeof(TModel);
            GetProperty(type, name).SetValue(instance, value);
        }

        private void SetField(string name, object value)
        {
            Type type = typeof(TModel);
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
