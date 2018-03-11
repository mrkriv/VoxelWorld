using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GameCore.Services
{
    public class DependencyInjection
    {
        private readonly Dictionary<Type, TypeImpl> _services = new Dictionary<Type, TypeImpl>();

        private class TypeImpl
        {
            public bool IsSinglton { get; set; }
            public object Instance { get; set; }
            public Type ImplType { get; set; }
        };
        
        public DependencyInjection()
        {
            AddSinglton(this);
        }

        public void AddSinglton<T>(T instance) where T : class
        {
            AddSinglton<T, T>(instance);
        }

        public void AddSinglton<T>() where T : class
        {
            AddSinglton<T, T>();
        }

        public void AddSinglton<TBase, TImpl>(TImpl instance)
            where TBase : class
            where TImpl : TBase
        {
            if (_services.ContainsKey(typeof(TBase)))
            {
                _services.Remove(typeof(TBase));
            }

            _services.Add(typeof(TBase), new TypeImpl
            {
                IsSinglton = true,
                ImplType = typeof(TImpl),
                Instance = instance
            });
        }
        
        public void AddSinglton<TBase, TImpl>()
            where TBase : class
            where TImpl : TBase
        {
            if (_services.ContainsKey(typeof(TBase)))
            {
                _services.Remove(typeof(TBase));
            }

            _services.Add(typeof(TBase), new TypeImpl
            {
                IsSinglton = true,
                ImplType = typeof(TImpl)
            });
        }
        
        public void AddTransient<T>() where T : class
        {
            AddTransient<T, T>();
        }

        public void AddTransient<TBase, TImpl>()
            where TBase : class
            where TImpl : TBase
        {
            if (_services.ContainsKey(typeof(TBase)))
            {
                _services.Remove(typeof(TBase));
            }

            _services.Add(typeof(TBase), new TypeImpl
            {
                IsSinglton = false,
                ImplType = typeof(TImpl)
            });
        }

        public T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }

        public object GetService(Type type)
        {
            if (!_services.ContainsKey(type))
                return null;

            var tuple = _services[type];

            if (tuple.IsSinglton)
            {
                if (tuple.Instance == null)
                {
                    tuple.Instance = New(tuple.ImplType);
                }

                return tuple.Instance;
            }

            return New(tuple.ImplType);
        }

        private T New<T>() where T : class
        {
            return New(typeof(T)) as T;
        }

        private object New(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length != 1)
            {
                throw new NotSupportedException($"Тип {type.FullName} должен содержать только один конструктор для создания через DI");
            }

            var ctor = constructors.Single();
            var ctorParams = new List<object>();

            foreach (var parameter in ctor.GetParameters())
            {
                var serv = GetService(parameter.ParameterType);
                if (serv == null)
                    throw new InvalidExpressionException($"Сервис {parameter.ParameterType.FullName} не зарегистрирован в DI");

                ctorParams.Add(serv);
            }

            return ctor.Invoke(ctorParams.ToArray());
        }
    }
}