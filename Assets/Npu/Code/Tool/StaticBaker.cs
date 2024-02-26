using System.Collections;
using System.Linq;
using UnityEngine;


namespace Npu
{
    public class StaticBaker : MonoBehaviour
    {
        public bool enable;
        public bool enable2;

        Mesh combinedMesh;

        private void Awake()
        {
            if (!enable) return;

            // Logger._Log<StaticBaker>(gameObject, $"Will Bake: {transform.parent.gameObject.name}/{gameObject.name}");
            // this.WaitForFrames(Random.Range(60, 120), () =>
            // {
            //     if (gameObject) Bake();
            // });
        }

        private void Start()
        {
            if (!enable2) return;

            StartCoroutine(DelayedBake());
        }

        private void OnDestroy()
        {
            DestroyCombinedMesh();
        }

        IEnumerator DelayedBake()
        {
            yield return new WaitForEndOfFrame();
            Bake();
        }

        [ContextMenu("Bake")]
        public void Bake()
        {
            DestroyCombinedMesh();

            Logger._Log<StaticBaker>(gameObject, $"Baking: {(transform.parent ? transform.parent.gameObject.name : "")}/{gameObject.name}");
            StaticBatchingUtility.Combine(gameObject);

            var mfs = gameObject.GetComponentsInChildren<MeshFilter>();
            var meshes = mfs.Select(mf => mf.sharedMesh).Where(m => m).GroupBy(m => m).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();
            var possibleMesh = meshes.Find(m => m && m.name.StartsWith("Combined Mesh"));
            combinedMesh = possibleMesh;
        }

        void DestroyCombinedMesh()
        {
            if (combinedMesh)
            {
                DestroyImmediate(combinedMesh);
                combinedMesh = null;
            }
        }
    }
}