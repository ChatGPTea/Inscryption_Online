using FunkyCode.LightTilemapCollider;
using FunkyCode.Rendering.Light.Shadow;
using UnityEngine;
using Collider = FunkyCode.SuperTilemapEditorSupport.Light.Shadow.Collider;
using Grid = FunkyCode.SuperTilemapEditorSupport.Light.Shadow.Grid;

namespace FunkyCode.Rendering.Light
{
    public static class NoSort
    {
        public static void Draw(Pass pass)
        {
            if (pass.drawShadows) Shadows.Draw(pass);

            if (pass.drawMask) Masks.Draw(pass);
        }

        public static class Shadows
        {
            public static void Draw(Pass pass)
            {
                switch (ShadowEngine.drawMode)
                {
                    case ShadowEngine.DRAW_MODE_SPRITEPROJECTION:

                        DrawCollider(pass);

                        break;

                    case ShadowEngine.DRAW_MODE_FAST:

                        ShadowEngine.GetMaterial().SetPass(0);

                        GL.Begin(GL.QUADS);

                        GL.Color(new Color(1, 0, 1, 1));

                        DrawCollider(pass);
                        DrawTilemapCollider(pass);

                        GL.End();

                        break;

                    default:

                        ShadowEngine.GetMaterial().SetPass(0);

                        GL.Begin(GL.TRIANGLES);

                        GL.Color(new Color(1, 0, 1, 1));

                        DrawCollider(pass);
                        DrawTilemapCollider(pass);

                        GL.End();

                        break;
                }
            }

            private static void DrawCollider(Pass pass)
            {
                var colliderCount = pass.layerShadowList.Count;

                if (colliderCount < 1) return;

                for (var id = 0; id < colliderCount; id++)
                {
                    var collider = pass.layerShadowList[id];

                    if (collider.ShadowDisabled()) continue;

                    switch (ShadowEngine.drawMode)
                    {
                        case ShadowEngine.DRAW_MODE_SPRITEPROJECTION:

                            ShadowEngine.spriteProjection = collider.mainShape.spriteShape.GetOriginalSprite();

                            var spriteRenderer = collider.mainShape.spriteShape.GetSpriteRenderer();
                            ShadowEngine.flipX = spriteRenderer.flipX;
                            ShadowEngine.flipY = spriteRenderer.flipY;

                            break;
                    }

                    Shadow.Shape.Draw(pass.light, collider);
                }
            }

            private static void DrawTilemapCollider(Pass pass)
            {
                for (var id = 0; id < pass.tilemapShadowList.Count; id++)
                {
                    var tilemap = pass.tilemapShadowList[id];

                    var shadowsDisabled = tilemap.ShadowsDisabled();

                    if (shadowsDisabled) continue;

                    if (!tilemap.InLight(pass.light)) continue;

                    switch (tilemap.mapType)
                    {
                        case MapType.UnityRectangle:
                        case MapType.UnityIsometric:
                        case MapType.UnityHexagon:

                            var baseTilemap = tilemap.GetCurrentTilemap();

                            switch (baseTilemap.shadowType)
                            {
                                case ShadowType.SpritePhysicsShape:
                                case ShadowType.Grid:

                                    Shadow.UnityTilemap.Draw(pass.light, tilemap);

                                    break;

                                case ShadowType.CompositeCollider:

                                    TilemapCollider.Rectangle.Draw(pass.light, tilemap);

                                    break;
                            }

                            break;

                        case MapType.SuperTilemapEditor:

                            switch (tilemap.superTilemapEditor.shadowTypeSTE)
                            {
                                case SuperTilemapEditorSupport.TilemapCollider.ShadowType.Grid:
                                case SuperTilemapEditorSupport.TilemapCollider.ShadowType.TileCollider:

                                    Grid.Draw(pass.light, tilemap);

                                    break;

                                case SuperTilemapEditorSupport.TilemapCollider.ShadowType.Collider:

                                    Collider.Draw(pass.light, tilemap);

                                    break;
                            }

                            break;
                    }
                }
            }
        }

        private static class Masks
        {
            public static void Draw(Pass pass)
            {
                var maskMaterial = pass.materialMask;
                maskMaterial.mainTexture = null;
                maskMaterial.SetPass(0);

                GL.Begin(GL.TRIANGLES);

                DrawCollider(pass);

                DrawTilemapCollider(pass);

                GL.End();

                DrawMesh(pass);

                DrawSprite(pass);

                DrawTilemapSprite(pass);
            }

            private static void DrawCollider(Pass pass)
            {
                var colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) return;

                for (var id = 0; id < colliderCount; id++)
                {
                    var collider = pass.layerMaskList[id];

                    switch (collider.mainShape.maskType)
                    {
                        case LightCollider2D.MaskType.SpritePhysicsShape:
                        case LightCollider2D.MaskType.CompositeCollider2D:
                        case LightCollider2D.MaskType.Collider2D:
                        case LightCollider2D.MaskType.Collider3D:

                            Shape.Mask(pass.light, collider, pass.layer);

                            break;
                    }
                }
            }

            private static void DrawMesh(Pass pass)
            {
                var colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) return;

                for (var id = 0; id < colliderCount; id++)
                {
                    var collider = pass.layerMaskList[id];

                    switch (collider.mainShape.maskType)
                    {
                        case LightCollider2D.MaskType.MeshRenderer:

                            Mesh.Mask(pass.light, collider, pass.materialMask, pass.layer);

                            break;

                        case LightCollider2D.MaskType.BumpedMeshRenderer:

                            var material = collider.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight,
                                pass.materialNormalMap_ObjectToLight);
                            Mesh.MaskNormalMap(pass.light, collider, material, pass.layer);

                            break;

                        case LightCollider2D.MaskType.SkinnedMeshRenderer:

                            SkinnedMesh.Mask(pass.light, collider, pass.materialMask, pass.layer);

                            break;
                    }
                }
            }

            private static void DrawSprite(Pass pass)
            {
                var colliderCount = pass.layerMaskList.Count;

                if (colliderCount < 1) return;

                SpriteRenderer2D.currentTexture = null;

                for (var id = 0; id < colliderCount; id++)
                {
                    var collider = pass.layerMaskList[id];

                    if (collider.mainShape.maskType != LightCollider2D.MaskType.Sprite) continue;

                    SpriteRenderer2D.Mask(pass.light, collider, pass.materialMask, pass.layer);
                }

                if (SpriteRenderer2D.currentTexture != null)
                {
                    GL.End();

                    pass.materialMask.mainTexture = null;

                    SpriteRenderer2D.currentTexture = null;
                }

                // pptimize bumped sprites batching? (use SpriteRenderer2D.currentTexture?)

                for (var id = 0; id < colliderCount; id++)
                {
                    var collider = pass.layerMaskList[id];

                    if (collider.mainShape.maskType != LightCollider2D.MaskType.BumpedSprite) continue;

                    var material = collider.bumpMapMode.SelectMaterial(pass.materialNormalMap_PixelToLight,
                        pass.materialNormalMap_ObjectToLight);

                    SpriteRenderer2D.MaskBumped(pass.light, collider, material, pass.layer);
                }
            }

            private static void DrawTilemapCollider(Pass pass)
            {
                for (var id = 0; id < pass.tilemapMaskList.Count; id++)
                {
                    var tilemap = pass.tilemapMaskList[id];

                    if (tilemap.maskLayer != pass.layerID) continue;

                    if (tilemap.MasksDisabled()) continue;

                    if (!tilemap.InLight(pass.light)) continue;

                    // Tilemap In Range
                    switch (tilemap.mapType)
                    {
                        case MapType.UnityRectangle:
                        case MapType.UnityIsometric:
                        case MapType.UnityHexagon:

                            var baseTilemap = tilemap.GetCurrentTilemap();

                            switch (baseTilemap.maskType)
                            {
                                case MaskType.Grid:
                                case MaskType.SpritePhysicsShape:

                                    UnityTilemap.MaskShape(pass.light, tilemap, pass.layer);

                                    break;
                            }

                            break;

                        case MapType.SuperTilemapEditor:

                            SuperTilemapEditorSupport.Light.Mask.Grid.Draw(pass.light, tilemap);

                            break;
                    }
                }
            }

            private static void DrawTilemapSprite(Pass pass)
            {
                for (var id = 0; id < pass.tilemapMaskList.Count; id++)
                {
                    var tilemap = pass.tilemapMaskList[id];

                    if (tilemap.maskLayer != pass.layerID) continue;

                    if (tilemap.MasksDisabled()) continue;

                    if (!tilemap.InLight(pass.light)) continue;

                    // Tilemap In Range

                    switch (tilemap.mapType)
                    {
                        case MapType.UnityRectangle:
                        case MapType.UnityIsometric:
                        case MapType.UnityHexagon:

                            var baseTilemap = tilemap.GetCurrentTilemap();

                            switch (baseTilemap.maskType)
                            {
                                case MaskType.Sprite:

                                    UnityTilemap.Sprite(pass.light, tilemap, pass.materialMask, pass.layer);

                                    break;

                                case MaskType.BumpedSprite:

                                    var material = tilemap.bumpMapMode.SelectMaterial(
                                        pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);

                                    UnityTilemap.BumpedSprite(pass.light, tilemap, material, pass.layer);

                                    break;
                            }

                            break;

                        case MapType.SuperTilemapEditor:

                            switch (tilemap.superTilemapEditor.maskTypeSTE)
                            {
                                case SuperTilemapEditorSupport.TilemapCollider.MaskType.Sprite:

                                    SuperTilemapEditorSupport.Light.Mask.SpriteRenderer2D.Sprite(pass.light, tilemap,
                                        pass.materialMask);

                                    break;

                                case SuperTilemapEditorSupport.TilemapCollider.MaskType.BumpedSprite:

                                    var material = tilemap.bumpMapMode.SelectMaterial(
                                        pass.materialNormalMap_PixelToLight, pass.materialNormalMap_ObjectToLight);

                                    SuperTilemapEditorSupport.Light.Mask.SpriteRenderer2D.BumpedSprite(pass.light,
                                        tilemap, material);

                                    break;
                            }

                            break;
                    }
                }
            }
        }
    }
}