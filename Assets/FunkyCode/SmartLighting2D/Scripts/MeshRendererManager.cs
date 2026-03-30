using FunkyCode.LightingSettings;
using UnityEngine;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class MeshRendererManager
    {
        // management
        public static LightingMeshRenderer AddBuffer(Object source)
        {
            var meshRenderer = new GameObject("Mesh Renderer (Id :" + (LightingMeshRenderer.GetCount() + 1) + ")");
            meshRenderer.transform.parent = LightingManager2D.Get().transform;

            if (Lighting2D.ProjectSettings.managerInternal == ManagerInternal.HideInHierarchy)
                meshRenderer.hideFlags = HideFlags.HideInHierarchy;
            else
                meshRenderer.hideFlags = HideFlags.None;

            var LightBuffer2D = meshRenderer.AddComponent<LightingMeshRenderer>();
            LightBuffer2D.Initialize();
            LightBuffer2D.owner = source;

            return LightBuffer2D;
        }

        public static LightingMeshRenderer Pull(Object source)
        {
            var sameExists = LightingMeshRenderer.List.Find(x => x.owner == source);
            if (sameExists)
            {
                sameExists.gameObject.SetActive(true);

                return sameExists;
            }

            var freeExists = LightingMeshRenderer.List.Find(x => x.Free);
            if (freeExists)
            {
                freeExists.owner = source;
                freeExists.gameObject.SetActive(true);

                return freeExists;
            }

            return AddBuffer(source);
        }
    }
}