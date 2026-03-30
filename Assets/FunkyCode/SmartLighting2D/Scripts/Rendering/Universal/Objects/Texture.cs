using UnityEngine;

namespace FunkyCode.Rendering.Universal
{
    public class Texture : Base
    {
        public static class Quad
        {
            private static Vector2 v0, v1, v2, v3;

            public static void Draw(Material material, Vector2 pos, Vector2 size, float rot, float z)
            {
                rot = rot * Mathf.Deg2Rad - Mathf.PI;

                var cos = Mathf.Cos(rot);
                var sin = Mathf.Sin(rot);

                var cosx = size.x * cos;
                var sinx = size.x * sin;

                var cosy = size.y * cos;
                var siny = size.y * sin;

                material.SetPass(0);

                GL.Begin(GL.QUADS);

                GL.Color(GLExtended.color);

                GL.TexCoord3(1, 1, 0);
                GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, z);

                GL.TexCoord3(0, 1, 0);
                GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, z);

                GL.TexCoord3(0, 0, 0);
                GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, z);

                GL.TexCoord3(1, 0, 0);
                GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, z);

                GL.End();
            }

            public static void Draw(Vector2 pos, Vector2 size, float rot)
            {
                rot = rot * Mathf.Deg2Rad - Mathf.PI;

                var cos = Mathf.Cos(rot);
                var sin = Mathf.Sin(rot);

                var cosx = size.x * cos;
                var sinx = size.x * sin;

                var cosy = size.y * cos;
                var siny = size.y * sin;

                GL.Begin(GL.QUADS);

                GL.TexCoord3(1, 1, 0);
                GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, 0);

                GL.TexCoord3(0, 1, 0);
                GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, 0);

                GL.TexCoord3(0, 0, 0);
                GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, 0);

                GL.TexCoord3(1, 0, 0);
                GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, 0);

                GL.End();
            }

            public static class Sprite
            {
                public static void DrawMultiPass(Vector2 pos, Vector2 size, Rect uv, float rot)
                {
                    rot = rot * Mathf.Deg2Rad + Mathf.PI;

                    var cos = Mathf.Cos(rot);
                    var sin = Mathf.Sin(rot);

                    var cosx = size.x * cos;
                    var sinx = size.x * sin;

                    var cosy = size.y * cos;
                    var siny = size.y * sin;

                    GL.Color(GLExtended.color);

                    GL.MultiTexCoord3(0, uv.width, uv.height, 0);
                    GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, 0);

                    GL.MultiTexCoord3(0, uv.x, uv.height, 0);
                    GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, 0);

                    GL.MultiTexCoord3(0, uv.x, uv.y, 0);
                    GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, 0);

                    GL.MultiTexCoord3(0, uv.width, uv.y, 0);
                    GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, 0);
                }

                public static void DrawPass(Vector2 pos, Vector2 size, Rect uv, float rot)
                {
                    rot = rot * Mathf.Deg2Rad + Mathf.PI;

                    var cos = Mathf.Cos(rot);
                    var sin = Mathf.Sin(rot);

                    var cosx = size.x * cos;
                    var sinx = size.x * sin;

                    var cosy = size.y * cos;
                    var siny = size.y * sin;

                    var uvX = uv.x;
                    var uvY = uv.y;
                    var uvWidth = uv.width;
                    var uvHeight = uv.height;

                    GL.Color(GLExtended.color);

                    GL.TexCoord3(uvWidth, uvHeight, 0);
                    GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, 0);

                    GL.TexCoord3(uvX, uvHeight, 0);
                    GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, 0);

                    GL.TexCoord3(uvX, uvY, 0);
                    GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, 0);

                    GL.TexCoord3(uvWidth, uvY, 0);
                    GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, 0);
                }

                public static void Draw(Vector2 pos, Vector2 size, Rect uv, float rot)
                {
                    rot = rot * Mathf.Deg2Rad - Mathf.PI;

                    var cos = Mathf.Cos(rot);
                    var sin = Mathf.Sin(rot);

                    var cosx = size.x * cos;
                    var sinx = size.x * sin;

                    var cosy = size.y * cos;
                    var siny = size.y * sin;

                    GL.Begin(GL.QUADS);

                    GL.Color(GLExtended.color);

                    GL.TexCoord3(uv.width, uv.height, 0);
                    GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, 0);

                    GL.TexCoord3(uv.x, uv.height, 0);
                    GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, 0);


                    GL.TexCoord3(uv.x, uv.y, 0);
                    GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, 0);

                    GL.TexCoord3(uv.width, uv.y, 0);
                    GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, 0);

                    GL.End();
                }
            }

            // ste uv (improve sprite matrix!)
            public static class STE
            {
                private static Vector2 v0, v1, v2, v3;

                public static void DrawPass(Vector2 pos, Vector2 size, Rect uv, float rot)
                {
                    var cos = Mathf.Cos(rot);
                    var sin = Mathf.Sin(rot);

                    var cosx = size.x * cos;
                    var sinx = size.x * sin;

                    var cosy = size.y * cos;
                    var siny = size.y * sin;

                    GL.TexCoord3(uv.x, uv.y, 0);
                    GL.Vertex3(-cosx + siny + pos.x, -sinx - cosy + pos.y, 0);

                    GL.TexCoord3(uv.x + uv.width, uv.y, 0);
                    GL.Vertex3(cosx + siny + pos.x, sinx - cosy + pos.y, 0);

                    GL.TexCoord3(uv.x + uv.width, uv.y + uv.height, 0);
                    GL.Vertex3(cosx - siny + pos.x, sinx + cosy + pos.y, 0);

                    GL.TexCoord3(uv.x, uv.y + uv.height, 0);
                    GL.Vertex3(-cosx - siny + pos.x, -sinx + cosy + pos.y, 0);
                }
            }
        }
    }
}