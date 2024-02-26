using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Npu.Helper
{

    public static class ComponentExtensions
    {
        public static List<MonoBehaviour> GetGenericComponents(this GameObject gameObject, Type genericType)
        {
            return gameObject.GetComponents<MonoBehaviour>()
                .Where(i => i.GetType().IsOfGenericType(genericType))
                .ToList();
        }
        
        public static List<MonoBehaviour> GetGenericComponentsInChildren(this GameObject gameObject, Type genericType, bool includeInactive=false)
        {
            return gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive)
                .Where(i => i.GetType().IsOfGenericType(genericType))
                .ToList();
        }
        
        public static MonoBehaviour GetGenericComponent(this GameObject gameObject, Type genericType)
        {
            return gameObject.GetComponents<MonoBehaviour>()
                .FirstOrDefault(i => i.GetType().IsOfGenericType(genericType));
        }
    }

}
