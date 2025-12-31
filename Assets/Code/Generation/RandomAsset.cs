using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Generation
{
    public class RandomAsset : MonoBehaviour
    {
        [Header("Variants (prefabs must have MeshFilter/MeshRenderer)")]
        public List<GameObject> variants;

        [Header("Optional Weighting")]
        public bool useWeights;
        public List<float> weights;

        [Header("Spawn Options")]
        public bool spawnOnAwake = true;

        [Header("Editor Preview")]
        public bool previewInEditor = true;
        [Tooltip("Set to -1 for random, 0..N to pick a specific variant")]
        public int previewIndex = -1;

        private int _currentPreviewIndex = -1;
        private GameObject _childInstance;

        private void Awake()
        {
            if (spawnOnAwake)
                Spawn();
        }

        private void Spawn()
        {
            if (variants == null || variants.Count == 0) return;

            var chosen = useWeights ? ChooseWeighted() : ChooseRandom();

            if (_childInstance == null)
            {
                // Spawn the child first time
                _childInstance = Instantiate(chosen, transform.position, transform.rotation, transform);
                _childInstance.transform.localPosition = Vector3.zero;
                _childInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // Reuse existing child by swapping mesh/renderer
                SwapMesh(_childInstance, chosen);
            }
        }

        private static void SwapMesh(GameObject target, GameObject source)
        {
            var targetFilter = target.GetComponent<MeshFilter>();
            var sourceFilter = source.GetComponent<MeshFilter>();
            if (targetFilter != null && sourceFilter != null)
                targetFilter.sharedMesh = sourceFilter.sharedMesh;

            var targetRenderer = target.GetComponent<MeshRenderer>();
            var sourceRenderer = source.GetComponent<MeshRenderer>();
            if (targetRenderer != null && sourceRenderer != null)
            {
                targetRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
            }
        }

        private GameObject ChooseRandom()
        {
            return variants[Random.Range(0, variants.Count)];
        }

        private GameObject ChooseWeighted()
        {
            if (weights.Count != variants.Count)
                return ChooseRandom();

            var total = weights.Sum(w => Mathf.Max(0, w));
            var roll = Random.value * total;

            for (var i = 0; i < variants.Count; i++)
            {
                roll -= Mathf.Max(0, weights[i]);
                if (roll <= 0) return variants[i];
            }

            return variants[^1];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying && previewInEditor)
            {
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                        UpdatePreview();
                };
            }
        }

        private void UpdatePreview()
        {
            if (variants == null || variants.Count == 0) return;
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject)) return;

            _currentPreviewIndex = previewIndex >= 0
                ? Mathf.Clamp(previewIndex, 0, variants.Count - 1)
                : Random.Range(0, variants.Count);

            if (_childInstance == null)
            {
                _childInstance = (GameObject)PrefabUtility.InstantiatePrefab(variants[_currentPreviewIndex], transform);
                _childInstance.transform.localPosition = Vector3.zero;
                _childInstance.transform.localRotation = Quaternion.identity;
                _childInstance.hideFlags = HideFlags.DontSave;
            }
            else
            {
                SwapMesh(_childInstance, variants[_currentPreviewIndex]);
            }
        }
#endif
    }
}
