using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Day
{
    public static class Shadow
    {
        public static float direction;
        public static float directionCos;
        public static float directionSin;
        public static float shadowDistance;

        public static Vector2 pointA, pointB, pointAOffset, pointBOffset;

        public static void Begin()
        {
            var material = Lighting2D.Materials.shadow.GetDayCPUShadow();
            material.SetColor("_Darkness", Lighting2D.DayLightingSettings.ShadowColor);

            material.SetPass(0);

            GL.Begin(GL.QUADS);

            direction = -Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;
            shadowDistance = Lighting2D.DayLightingSettings.height;

            directionCos = Mathf.Cos(direction);
            directionSin = Mathf.Sin(direction);
        }

        public static void End()
        {
            GL.End();
        }

        public static void Draw(DayLightCollider2D id, Vector2 position)
        {
            if (id.mainShape.height <= 0 || id.shadowTranslucency >= 1) return;

            if (!id.InAnyCamera()) return;

            var distance = shadowDistance * id.mainShape.height;
            var cosShadow = directionCos * distance;
            var sinShadow = directionSin * distance;

            var shape = id.mainShape;

            // if (!shape.isStatic)
            // {
            //    shape.ResetWorld();//????????????
            // }

            var polygons = shape.GetPolygonsWorld();

            var pos = position;

            var polygonCount = polygons.Count;

            var softness = id.shadowSoftness > 0 && id.shadowEffect == DayLightCollider2D.ShadowEffect.Softness;

            var falloff = id.shadowEffect == DayLightCollider2D.ShadowEffect.Falloff ? 2 : 1;

            for (var p = 0; p < polygonCount; p++)
            {
                var polygon = polygons[p];

                var pointsCount = polygon.points.Length;

                for (var i = 0; i < pointsCount; i++)
                {
                    pointA = polygon.points[i];
                    pointA.x += pos.x;
                    pointA.y += pos.y;

                    pointB = polygon.points[(i + 1) % pointsCount];
                    pointB.x += pos.x;
                    pointB.y += pos.y;

                    pointAOffset.x = pointA.x + cosShadow;
                    pointAOffset.y = pointA.y + sinShadow;

                    pointBOffset.x = pointB.x + cosShadow;
                    pointBOffset.y = pointB.y + sinShadow;

                    // add: pass coordinates via color
                    // add: projection via vertex shader
                    // color(px, py, nx, ny)
                    // gl.tex(translucency, 0, 0)
                    // shader _direction + cossinn values
                    // gl.vertex3(0, 0, -1) -1 project

                    if (id.shadowDistance > 0)
                    {
                        GL.Color(new Color(0, 0, falloff, id.shadowTranslucency));

                        GL.Vertex3(pointA.x, pointA.y, 0);
                        GL.Vertex3(pointAOffset.x, pointAOffset.y, 1);
                        GL.Vertex3(pointBOffset.x, pointBOffset.y, 1);
                        GL.Vertex3(pointB.x, pointB.y, 0);
                    }

                    if (softness)
                    {
                        // only when soft
                        DrawLine(pointAOffset, pointBOffset, 0, id.shadowTranslucency, id.shadowSoftness);
                        DrawLine(pointA, pointAOffset, 0, id.shadowTranslucency, id.shadowSoftness);
                        DrawLine(pointBOffset, pointB, 0, id.shadowTranslucency, id.shadowSoftness);
                        DrawLine(pointA, pointB, 0, id.shadowTranslucency, id.shadowSoftness);

                        DrawLine(pointA, pointA, 1, id.shadowTranslucency, id.shadowSoftness);
                        DrawLine(pointAOffset, pointAOffset, 1, id.shadowTranslucency, id.shadowSoftness);
                    }
                }
            }
        }

        public static void DrawLine(Vector2 point, Vector2 nextPoint, int type, float translucency, float softness)
        {
            var sizePoint = softness;
            var sizePointNext = softness;

            var direction = point.Atan2(nextPoint);

            var p1 = point;
            var p2 = nextPoint;
            var p3 = nextPoint;
            var p4 = point;

            switch (type)
            {
                case 0:

                    p3.x -= Mathf.Cos(direction + Mathf.PI / 2) * sizePointNext;
                    p3.y -= Mathf.Sin(direction + Mathf.PI / 2) * sizePointNext;

                    p4.x -= Mathf.Cos(direction + Mathf.PI / 2) * sizePoint;
                    p4.y -= Mathf.Sin(direction + Mathf.PI / 2) * sizePoint;

                    GL.Color(new Color(1, 0, 0, translucency));
                    GL.Vertex3(p1.x, p1.y, 0);
                    GL.Vertex3(p2.x, p2.y, 0);
                    GL.Vertex3(p3.x, p3.y, 1);
                    GL.Vertex3(p4.x, p4.y, 1);

                    break;

                case 1:

                    GL.Color(new Color(0, 1, 0, translucency));
                    GL.Vertex3(nextPoint.x - sizePointNext, nextPoint.y - sizePointNext, 0);
                    GL.Vertex3(nextPoint.x + sizePointNext, nextPoint.y - sizePointNext, 1);
                    GL.Vertex3(nextPoint.x + sizePointNext, nextPoint.y + sizePointNext, 2);
                    GL.Vertex3(nextPoint.x - sizePointNext, nextPoint.y + sizePointNext, 3);

                    break;
            }
        }

        public static void DrawLineTri(Vector2 point, Vector2 nextPoint, int type, float translucency, float softness)
        {
            var sizePoint = softness;
            var sizePointNext = softness;

            var direction = point.Atan2(nextPoint);

            var p1 = point;
            var p2 = nextPoint;
            var p3 = nextPoint;
            var p4 = point;

            switch (type)
            {
                case 0:

                    p3.x -= Mathf.Cos(direction + Mathf.PI / 2) * sizePointNext;
                    p3.y -= Mathf.Sin(direction + Mathf.PI / 2) * sizePointNext;

                    p4.x -= Mathf.Cos(direction + Mathf.PI / 2) * sizePoint;
                    p4.y -= Mathf.Sin(direction + Mathf.PI / 2) * sizePoint;

                    GL.Color(new Color(1, 0, 0, translucency));
                    GL.Vertex3(p1.x, p1.y, 0);
                    GL.Vertex3(p2.x, p2.y, 0);
                    GL.Vertex3(p3.x, p3.y, 1);

                    GL.Vertex3(p3.x, p3.y, 1);
                    GL.Vertex3(p4.x, p4.y, 1);
                    GL.Vertex3(p1.x, p1.y, 0);

                    break;

                case 1:

                    GL.Color(new Color(0, 1, 0, translucency));
                    GL.Vertex3(nextPoint.x - sizePointNext, nextPoint.y - sizePointNext, 0);
                    GL.Vertex3(nextPoint.x + sizePointNext, nextPoint.y - sizePointNext, 1);
                    GL.Vertex3(nextPoint.x + sizePointNext, nextPoint.y + sizePointNext, 2);

                    GL.Vertex3(nextPoint.x + sizePointNext, nextPoint.y + sizePointNext, 2);
                    GL.Vertex3(nextPoint.x - sizePointNext, nextPoint.y + sizePointNext, 3);
                    GL.Vertex3(nextPoint.x - sizePointNext, nextPoint.y - sizePointNext, 0);

                    break;
            }
        }

        public static void DrawFill(DayLightCollider2D id, Vector2 position)
        {
            if (!id.InAnyCamera()) return;

            GLExtended.color = new Color(0, 0, 1, id.shadowTranslucency);

            Vector2 pos = id.mainShape.transform.position;
            pos.x += position.x;
            pos.y += position.y;

            var scale = id.mainShape.transform2D.scale;
            var rotation = id.mainShape.transform2D.rotation;

            var shape = id.mainShape;

            if (!shape.isStatic) shape.ResetWorld();

            var meshes = shape.GetMeshes();

            if (meshes == null) return;

            if (meshes.Count < 1) return;

            GLExtended.DrawMeshPass(meshes, pos, scale, rotation);

            var softness = id.shadowSoftness > 0 && id.shadowEffect == DayLightCollider2D.ShadowEffect.Softness;

            if (softness)
            {
                var polygons = shape.GetPolygonsWorld();

                if (polygons == null) return;

                var polygonCount = polygons.Count;

                for (var p = 0; p < polygonCount; p++)
                {
                    var polygon = polygons[p];

                    var pointsCount = polygon.points.Length;

                    for (var i = 0; i < pointsCount; i++)
                    {
                        pointA = polygon.points[i];
                        pointA.x += position.x;
                        pointA.y += position.y;

                        pointB = polygon.points[(i + 1) % pointsCount];
                        pointB.x += position.x;
                        pointB.y += position.y;

                        DrawLineTri(pointA, pointB, 0, id.shadowTranslucency, id.shadowSoftness);
                        DrawLineTri(pointA, pointA, 1, id.shadowTranslucency, id.shadowSoftness);
                        DrawLineTri(pointA, pointA, 1, id.shadowTranslucency, id.shadowSoftness);
                    }
                }
            }
        }

        public static void DrawTilemap(DayLightTilemapCollider2D id, Vector2 position, Camera camera)
        {
            //if (id.InAnyCamera() == false) {
            //     continue;
            //}

            if (id.height <= 0)
            {
                // return;
            }

            var distance = shadowDistance * id.height;
            var cosShadow = directionCos * distance;
            var sinShadow = directionSin * distance;

            foreach (var dayTile in id.dayTiles)
            {
                if (!dayTile.InCamera(camera)) continue;

                var polygons = dayTile.polygons;

                foreach (var polygon in polygons)
                {
                    var pointsCount = polygon.points.Length;

                    for (var i = 0; i < pointsCount; i++)
                    {
                        pointA = polygon.points[i];
                        pointA.x += position.x;
                        pointA.y += position.y;

                        pointB = polygon.points[(i + 1) % pointsCount];
                        pointB.x += position.x;
                        pointB.y += position.y;

                        pointAOffset.x = pointA.x + cosShadow;
                        pointAOffset.y = pointA.y + sinShadow;

                        pointBOffset.x = pointB.x + cosShadow;
                        pointBOffset.y = pointB.y + sinShadow;

                        // soft shadows?
                        // translucency?

                        GL.Color(new Color(0, 0, 1, id.shadowTranslucency));

                        GL.Vertex3(pointA.x, pointA.y, 0);
                        GL.Vertex3(pointAOffset.x, pointAOffset.y, 0);
                        GL.Vertex3(pointBOffset.x, pointBOffset.y, 0);
                        GL.Vertex3(pointB.x, pointB.y, 0);

                        if (id.shadowSoftness > 0)
                        {
                            // only when soft
                            DrawLine(pointAOffset, pointBOffset, 0, id.shadowTranslucency, id.shadowSoftness);
                            DrawLine(pointA, pointAOffset, 0, id.shadowTranslucency, id.shadowSoftness);
                            DrawLine(pointBOffset, pointB, 0, id.shadowTranslucency, id.shadowSoftness);
                            DrawLine(pointA, pointB, 0, id.shadowTranslucency, id.shadowSoftness);

                            DrawLine(pointA, pointA, 1, id.shadowTranslucency, id.shadowSoftness);
                            DrawLine(pointAOffset, pointAOffset, 1, id.shadowTranslucency, id.shadowSoftness);
                        }
                    }
                }
            }
        }
    }
}