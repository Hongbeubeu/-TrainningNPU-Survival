using UnityEngine;
using System.Linq;

namespace Npu
{
    public class DataBinderFinder : MonoBehaviour
    {
        public Object target;
        public string path;
        public bool exactPath;

        DataBinder[] GetAllBinders()
        {
            return GetComponentsInChildren<DataBinder>(true).Concat(GetComponentsInParent<DataBinder>(true)).ToArray();
        }

        [ContextMenu("Find by target")]
        public void FindByTarget()
        {
            if (target)
            {
                var binders = GetAllBinders().Where(b => b.Targets?.Any(t => t.target?.Target == target) ?? false);
                foreach (var b in binders)
                {
                    Log(b);
                }
            }
        }

        [ContextMenu("Find by path")]
        public void FindByPath()
        {
            if (!string.IsNullOrEmpty(path))
            {
                var paths = path.Split(' ', ',', '.', '/').Select(p => p.ToLower()).ToList();
                var binders = GetAllBinders().Where(b => b.Targets?.Any(t =>
                {
                    var intersect = t.paths.Select(p => p.ToLower()).Intersect(paths);
                    return exactPath ? intersect.Count() >= paths.Count : intersect.Any();
                }) ?? false);
                foreach (var b in binders)
                {
                    Log(b);
                }
            }
        }

        void Log(DataBinder b)
        {
            Debug.Log(b, b);
        }
    }
}