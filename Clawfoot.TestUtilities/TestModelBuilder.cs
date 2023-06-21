#nullable enable
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
        private readonly List<Action> _actions = new List<Action>();
        private TModel? _instance;
        
        public TestModelBuilder() { }

        public TestModelBuilder(TModel instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance), "Cannot create a TestModelBuilder with a null instance through this constructor");
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

        public TestModelBuilder<TModel> With<TMember>(Expression<Func<TModel, TMember>> memberExpression,
            TMember value,
            MemberTypes memberType = MemberTypes.Property)
        {
            string memberName = ((MemberExpression)memberExpression.Body).Member.Name;
            _actions.Add(() => Set(memberName, value, memberType));
            return this;
        }
        
        public TestModelBuilder<TModel> With<TMember>(Expression<Func<TModel, TMember>> memberExpression,
            Func<TestModelBuilder<TMember>, TestModelBuilder<TMember>> builder,
            MemberTypes memberType = MemberTypes.Property)
            where TMember : class
        {
            TMember value = builder(new TestModelBuilder<TMember>()).Build();
            return With(memberExpression, value, memberType);
        }
        
        public TestModelBuilder<TModel> With<TMember>(Expression<Func<TModel, TMember>> memberExpression,
            Func<TestModelBuilder<TMember>, TMember> builder,
            MemberTypes memberType = MemberTypes.Property)
            where TMember : class
        {
            TMember value = builder(new TestModelBuilder<TMember>());
            return With(memberExpression, value, memberType);
        }

        public TestModelBuilder<TModel> With(string memberName, object value, MemberTypes memberType = MemberTypes.Property)
        {
            _actions.Add(() => Set(memberName, value, memberType));
            return this;
        }

        /// <summary>
        /// Builds the model
        /// </summary>
        /// <returns></returns>
        public TModel Build()
        {
            _instance = TryCreateInstance();
            
            foreach (Action action in _actions)
            {
                action.Invoke();
            }

            return _instance;
        }
        
        /// <summary>
        /// Builds the model using the specified derived type
        /// </summary>
        public TModel Build<TBuildWith>()
            where TBuildWith : class, TModel
        {
            _instance = TryCreateInstance<TBuildWith>();
            
            foreach (Action action in _actions)
            {
                action.Invoke();
            }

            return _instance;
        }


        //---------------------------------------------------------
        //===== Private Helpers =====
        
        private TModel TryCreateInstance()
        {
            Type modelType = typeof(TModel);

            if (modelType.IsInterface)
            {
                throw new InvalidOperationException("Cannot construct instance of interface. Please use the Build<TBuildWith> overload to specify a concrete type.");
            }
            
            if (_instance is null)
            {
                return (TModel)Activator.CreateInstance(typeof(TModel), true);
            }

            return _instance;
        }
        
        private TModel TryCreateInstance<TBuildWith>()
            where TBuildWith : class, TModel
        {
            if (_instance is null)
            {
                return (TModel)Activator.CreateInstance(typeof(TBuildWith), true)! ?? throw new Exception($"Failed to create instance of type {typeof(TBuildWith)}");
            }

            return _instance;
        }

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
            GetProperty(type, name).SetValue(_instance, value);
        }

        private void SetField(string name, object value)
        {
            Type type = typeof(TModel);
            GetField(type, name).SetValue(_instance, value);
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
