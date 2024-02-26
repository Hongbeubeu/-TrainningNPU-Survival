using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npu.Helper;
using UnityEngine;


namespace Npu.Formula
{
    public class ResolvableAttribute : Attribute
    {
    
    }
    
    [Serializable]
    public class ParameterResolver
    {
        [SerializeField] private string path;

        private List<ResolverStep> steps;
        
        public IParameter Resolve(ParameterManager manager)
        {
            

            return null;
        }

        private static string[] Properties(Type t) 
            => t.GetInstanceAttributedProperties<ResolvableAttribute>()
            .Select(i => i.property.Name).ToArray();

        private class ResolverStep
        {
            public Type type;
            public string property;

            public ResolverStep(Type type)
            {
                this.type = type;
            }
            
            private PropertyInfo Property { get; set; }
            public Type PropertyType => Property?.PropertyType; 

            public void Setup()
            {
                Property = type.GetProperty(property,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (Property == null)
                {
                    Logger.Error<ParameterResolver>($"No filed {property} of type {type}");
                }
            }

            public object Resolve(object target) => Property?.GetValue(target);

            public  string[] Properties 
                => type.GetInstanceAttributedProperties<ResolvableAttribute>()
                    .Select(i => i.property.Name).ToArray();
        }
    }
}