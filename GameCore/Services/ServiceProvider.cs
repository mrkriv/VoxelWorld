using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GameCore.Services
{
    public class ServiceProvider
    {
        private readonly Dictionary<Type, TypeImpl> _services = new Dictionary<Type, TypeImpl>();

        private class TypeImpl
        {
            public bool IsSinglton { get; set; }
            public object Instance { get; set; }
            public Type ImplType { get; set; }
        };
        
        public ServiceProvider()
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

            var implType = typeof(TImpl);
            if (typeof(TImpl).IsGenericTypeDefinition)
                implType = null;
            
            _services.Add(typeof(TBase), new TypeImpl
            {
                IsSinglton = true,
                ImplType = implType
            });
        }
        
        public void AddTransient<T>() where T : class
        {
            AddTransient(typeof(T), typeof(T));
        }

        public void AddTransient<TBase, TImpl>()
            where TBase : class
            where TImpl : TBase
        {
            AddTransient(typeof(TBase), typeof(TImpl));
        }

        public void AddTransient(Type type)
        {
            AddTransient(type, type);
        }

        public void AddTransient(Type typeBase, Type typeImplement)
        {
            if (_services.ContainsKey(typeBase))
            {
                _services.Remove(typeBase);
            }

            var implType = typeImplement;
            if (typeImplement.IsGenericTypeDefinition)
                implType = null;
            
            _services.Add(typeBase, new TypeImpl
            {
                IsSinglton = false,
                ImplType = implType
            });
        }

        private TypeImpl GetTypeImpl(Type type)
        {
            if (_services.TryGetValue(type, out var result))
                return result;

            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();
                if (_services.TryGetValue(genericDefinition, out result))
                    return result;
            }

            return null;
        }
        
        public bool ContainsService(Type type)
        {
            return GetTypeImpl(type) != null;
        }

        public T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }

        public object GetService(Type type)
        {
            var typeImpl = GetTypeImpl(type);

            if (typeImpl == null)
                throw new InvalidExpressionException($"Тип {type} не зарегистрирован в DI");

            var implType = typeImpl.ImplType ?? type;
            
            if (typeImpl.IsSinglton)
            {
                if (typeImpl.Instance == null)
                    typeImpl.Instance = New(implType);
                
                return typeImpl.Instance;
            }

            return New(implType);
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
                ctorParams.Add(serv);
            }

            return ctor.Invoke(ctorParams.ToArray());
        }
    }
}