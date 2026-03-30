using UnityEngine;

namespace FunkyCode.Rendering.Depth
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
            Lighting2D.Materials.shadow.GetDepthDayShadow().SetPass(0);

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
            if (id.mainShape.height <= 0) // id.shadowTranslucency >= 1
                return;

            if (!id.InAnyCamera()) return;

            var distance = shadowDistance * id.mainShape.height;
            var cosShadow = directionCos * distance;
            var sinShadow = directionSin * distance;

            var depth = (100f + id.GetDepth()) / 255;
            var depthFalloff = id.depthFalloff == DayLightCollider2D.DepthFalloff.Enabled;

            var color = new Color(depth, 0, 0, 1);
            var empty = new Color(0, 0, 0, 1);

            if (!depthFalloff) GL.Color(color);

            var shape = id.mainShape;

            if (!shape.isStatic) shape.ResetWorld();

            var polygons = shape.GetPolygonsWorld();

            var pos = position;

            var polygonCount = polygons.Count;

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

                    if (depthFalloff) GL.Color(color);

                    GL.Vertex3(pointB.x, pointB.y, 0);
                    GL.Vertex3(pointA.x, pointA.y, 0);

                    if (depthFalloff) GL.Color(empty);

                    GL.Vertex3(pointAOffset.x, pointAOffset.y, 0);
                    GL.Vertex3(pointBOffset.x, pointBOffset.y, 0);
                }
            }
        }

        public static void DrawFill(DayLightCollider2D id, Vector2 position)
        {
            if (id.mainShape.height <= 0) // id.shadowTranslucency >= 1
                return;

            if (!id.InAnyCamera()) return;

            var depth = (100f + id.GetDepth()) / 255;

            GLExtended.color = new Color(depth, 0, 0, 1);

            var pos = id.mainShape.transform2D.position;
            pos += position;
            var scale = id.mainShape.transform2D.scale;
            var rotation = id.mainShape.transform2D.rotation;

            var shape = id.mainShape;

            if (!shape.isStatic) shape.ResetWorld();

            var polygons = shape.GetPolygonsLocal();

            var meshes = shape.GetMeshes();

            if (meshes == null) return;

            if (meshes.Count < 1) return;

            GLExtended.DrawMeshPass(meshes, pos, scale, rotation);
        }
    }
}