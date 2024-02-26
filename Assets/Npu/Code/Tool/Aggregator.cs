using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Npu.tils
{
    public class Aggregator : MonoBehaviour
    {

        public int value;
        
        public void Sum(object[] values)
        {
            var intVs = values.OfType<int>().ToList();
            if (intVs.Any())
            {
                Logger.Log<Aggregator>($"SUM(int) = {intVs.Sum()}");
                return;
            }
            
            var floatVs = values.OfType<float>().ToList();
            if (floatVs.Any())
            {
                Logger.Log<Aggregator>($"SUM(float) = {floatVs.Sum()}");
                return;
            }
            
            var doubleVs = values.OfType<double>().ToList();
            if (doubleVs.Any())
            {
                Logger.Log<Aggregator>($"SUM(double) = {doubleVs.Sum()}");
                return;
            }
            
            Logger.Error<Aggregator>("Data Failed");
        }

        public void SetQueue(object[] values)
        {
            var mats = values.OfType<Material>();
            foreach (var mat in mats)
            {
                mat.renderQueue = value;
                Logger.Log<Aggregator>($"{mat.name} queue {value}");
#if UNITY_EDITOR
                EditorUtility.SetDirty(mat);          
#endif                
            }
        }
    }
}