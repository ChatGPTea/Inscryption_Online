using System.Collections.Generic;
using FunkyCode.LightingSettings;
using UnityEngine;
using UnityEngine.Rendering;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightingMeshRenderer : LightingMonoBehaviour
    {
        [SerializeField] public Object owner;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Material[] materials = new Material[1];
        [SerializeField] private MeshModeShader meshModeShader = MeshModeShader.Additive;

        public Mesh getSpriteMesh;

        public Mesh getMeshLight;
        private Material[] meshModeMaterial;
        public static List<LightingMeshRenderer> List { get; } = new();

        public bool Free => !owner;

        public void Reset()
        {
            owner = null;

            meshRenderer.enabled = false;

            if (meshRenderer.sharedMaterial) meshRenderer.sharedMaterial.mainTexture = null;
        }

        public void LateUpdate()
        {
            if (!owner)
            {
                Reset();
                return;
            }

            if (IsRendered())
            {
                meshRenderer.enabled = true;
            }
            else
            {
                Reset();
                meshRenderer.enabled = false;
            }
        }

        public void OnEnable()
        {
            List.Add(this);
        }

        public void OnDisable()
        {
            List.Remove(this);
        }

        public Material[] GetMaterials()
        {
            if (materials == null) materials = new Material[1];

            if (materials.Length < 1) materials = new Material[1];

            switch (meshModeShader)
            {
                case MeshModeShader.Additive:

                    if (!materials[0]) materials[0] = LightingMaterial.Load("Light2D/Internal/MeshModeAdditive").Get();

                    break;

                case MeshModeShader.Alpha:

                    if (!materials[0]) materials[0] = LightingMaterial.Load("Light2D/Internal/MeshModeAlpha").Get();

                    break;

                case MeshModeShader.Custom:

                    materials = meshModeMaterial;

                    break;
            }

            return materials;
        }

        public static int GetCount()
        {
            return List.Count;
        }

        public void Initialize()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();

            Debug.Log("initialize");

            // Mesh System?
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;
        }

        public bool IsRendered()
        {
            if (owner is Light2D light)
                return light.meshMode.enable && light.isActiveAndEnabled && light.InCameras();
            if (owner is LightSprite2D lightSprite)
                return lightSprite.meshMode.enable && lightSprite.isActiveAndEnabled;

            return false;
        }

        public void ClearMaterial()
        {
            materials = new Material[1];
        }

        public void UpdateLight(Light2D id, MeshMode meshMode)
        {
            // Camera
            if (meshModeMaterial != meshMode.materials)
            {
                meshModeMaterial = meshMode.materials;

                ClearMaterial();
            }

            if (meshModeShader != meshMode.shader)
            {
                meshModeShader = meshMode.shader;

                ClearMaterial();
            }

            var materials = GetMaterials();

            if (materials == null) return;

            if (id.IsPixelPerfect())
            {
                var camera = Camera.main;

                var cameraSize = LightingRender2D.GetSize(camera);
                var cameraPosition = LightingPosition.GetPosition2D(-camera.transform.position);

                transform.position = new Vector3(cameraPosition.x, cameraPosition.y, id.transform.position.z);

                transform.localScale = new Vector3(cameraSize.x, cameraSize.y, 1);
            }
            else
            {
                transform.position = id.transform.position;

                transform.localScale = new Vector3(id.size, id.size, 1);
            }

            transform.rotation = Quaternion.Euler(0, 0, 0);
            // transform.rotation = id.transform.rotation; // only if rotation enabled

            if (id.Buffer != null && meshRenderer)
            {
                var lightColor = id.color;
                lightColor.a = id.meshMode.alpha;

                for (var i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (!material)
                        continue;

                    material.color = lightColor;
                    material.SetColor("_Color", lightColor);
                    material.SetFloat("_Inverted", 1);

                    if (id.lightType == Light2D.LightType.Sprite)
                        material.SetTexture("_Sprite", id.GetSprite().texture);
                    else
                        material.SetTexture("_Sprite", null);

                    if (id.lightType == Light2D.LightType.FreeForm)
                    {
                        material.SetFloat("_Outer", 0);
                        material.SetFloat("_Inner", 360);

                        var tex = id.GetBuffer().freeFormTexture;
                        if (tex != null)
                            material.SetTexture("_Freeform", tex.renderTexture);
                        else
                            material.SetTexture("_Freeform", null);
                    }
                    else
                    {
                        material.SetFloat("_Outer", id.spotAngleOuter - id.spotAngleInner);
                        material.SetFloat("_Inner", id.spotAngleInner);
                        material.SetTexture("_Freeform", null);
                    }

                    material.SetTexture("_Lightmap", id.Buffer.renderTexture.renderTexture);
                    material.SetFloat("_Rotation", id.transform2D.rotation * 0.0174533f);
                }

                id.meshMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

                meshRenderer.sharedMaterials = GetMaterials();

                meshRenderer.enabled = true;

                meshFilter.mesh = GetMeshLight();
            }
        }

        public void UpdateLightSprite(LightSprite2D id, MeshMode meshMode)
        {
            if (!id.GetSprite())
            {
                Reset();
                return;
            }

            if (meshModeMaterial != meshMode.materials)
            {
                meshModeMaterial = meshMode.materials;

                ClearMaterial();
            }

            if (meshModeShader != meshMode.shader)
            {
                meshModeShader = meshMode.shader;

                ClearMaterial();
            }

            var material = GetMaterials();
            if (material == null) return;

            var rotation = id.lightSpriteTransform.rotation;
            if (id.lightSpriteTransform.applyRotation) rotation += id.transform.rotation.eulerAngles.z;

            ////////////////////// Scale
            var scale = Vector2.zero;

            var sprite = id.GetSprite();

            var spriteRect = sprite.textureRect;

            scale.x = sprite.texture.width / spriteRect.width;
            scale.y = sprite.texture.height / spriteRect.height;

            var size = id.lightSpriteTransform.scale;

            size.x *= 2;
            size.y *= 2;

            size.x /= scale.x;
            size.y /= scale.y;

            size.x *= sprite.texture.width / (sprite.pixelsPerUnit * 2);
            size.y *= sprite.texture.height / (sprite.pixelsPerUnit * 2);

            if (id.spriteRenderer.flipX) size.x = -size.x;

            if (id.spriteRenderer.flipY) size.y = -size.y;

            ////////////////////// PIVOT
            var rect = spriteRect;
            var pivot = sprite.pivot;

            pivot.x /= spriteRect.width;
            pivot.y /= spriteRect.height;
            pivot.x -= 0.5f;
            pivot.y -= 0.5f;


            pivot.x *= size.x;
            pivot.y *= size.y;


            var pivotDist = Mathf.Sqrt(pivot.x * pivot.x + pivot.y * pivot.y);
            var pivotAngle = Mathf.Atan2(pivot.y, pivot.x);

            var rot = rotation * Mathf.Deg2Rad + Mathf.PI;

            var position = Vector2.zero;

            // Pivot Pushes Position

            position.x += Mathf.Cos(pivotAngle + rot) * pivotDist * id.transform.lossyScale.x;
            position.y += Mathf.Sin(pivotAngle + rot) * pivotDist * id.transform.lossyScale.y;
            position.x += id.transform.position.x;
            position.y += id.transform.position.y;
            position.x += id.lightSpriteTransform.position.x;
            position.y += id.lightSpriteTransform.position.y;

            Vector3 pos = position;
            pos.z = id.transform.position.z - 0.1f;
            transform.position = pos;

            var scale2 = id.transform.lossyScale;

            scale2.x *= size.x;
            scale2.y *= size.y;

            scale2.x /= 2;
            scale2.y /= 2;

            scale2.z = 1;

            transform.localScale = scale2;
            transform.rotation = Quaternion.Euler(0, 0, rotation);

            var uvRect = new Rect();
            uvRect.x = rect.x / sprite.texture.width;
            uvRect.y = rect.y / sprite.texture.height;
            uvRect.width = rect.width / sprite.texture.width + uvRect.x;
            uvRect.height = rect.height / sprite.texture.height + uvRect.y;

            if (meshRenderer)
            {
                var lightColor = id.color;
                lightColor.a = id.meshMode.alpha;

                for (var i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (!mat)
                        continue;

                    mat.color = lightColor;
                    mat.SetColor("_Color", lightColor);
                    mat.SetFloat("_Inverted", 0);
                    mat.SetTexture("_Sprite", id.GetSprite().texture);
                    mat.SetTexture("_Lightmap", null);

                    mat.SetFloat("_Outer", 0);
                    mat.SetFloat("_Inner", 360);
                    mat.SetFloat("_Rotation", 0);
                }

                id.meshMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

                meshRenderer.sharedMaterials = materials;

                meshRenderer.enabled = true;

                var mesh = GetMeshSprite();

                var uvs = mesh.uv;
                uvs[0].x = uvRect.x;
                uvs[0].y = uvRect.y;

                uvs[1].x = uvRect.width;
                uvs[1].y = uvRect.y;

                uvs[2].x = uvRect.width;
                uvs[2].y = uvRect.height;

                uvs[3].x = uvRect.x;
                uvs[3].y = uvRect.height;

                mesh.uv = uvs;

                meshFilter.mesh = mesh;
            }
        }

        public Mesh GetMeshSprite()
        {
            if (getSpriteMesh)
                return getSpriteMesh;

            var mesh = new Mesh();

            mesh.vertices = new[] { new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1) };
            mesh.triangles = new[] { 2, 1, 0, 0, 3, 2 };
            mesh.uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

            getSpriteMesh = mesh;

            return getSpriteMesh;
        }

        public Mesh GetMeshLight()
        {
            if (getMeshLight)
                return getMeshLight;

            var mesh = new Mesh();

            mesh.vertices = new[] { new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1) };
            mesh.triangles = new[] { 2, 1, 0, 0, 3, 2 };
            mesh.uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

            getMeshLight = mesh;

            return getMeshLight;
        }
    }
}