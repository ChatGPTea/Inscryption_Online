using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode
{
    public class OcclusionMesh
    {
        public List<Color> colors;

        private Mesh mesh;

        public int tileCount;
        private readonly OcclusionTileset tileset;
        public List<int> triangles;
        public List<Vector2> uv;

        public List<Vector3> vertices;

        public OcclusionMesh(OcclusionTileset setTileset)
        {
            tileset = setTileset;

            mesh = new Mesh();

            vertices = new List<Vector3>();
            uv = new List<Vector2>();
            triangles = new List<int>();
            colors = new List<Color>();
        }

        public void AddTile(int id, Vector2Int tilePosition, OcclusionTileset.TileRotation tileRotation, Color color,
            bool flipX = false, bool flipY = false)
        {
            var uv0 = tileset.uv[id].uv0;
            var uv1 = tileset.uv[id].uv1;
            var uv2 = tileset.uv[id].uv2;
            var uv3 = tileset.uv[id].uv3;

            var tempUV0 = uv0;
            var tempUV1 = uv1;
            var tempUV2 = uv2;
            var tempUV3 = uv3;

            switch (tileRotation)
            {
                case OcclusionTileset.TileRotation.right:
                    uv0 = tempUV1;
                    uv1 = tempUV2;
                    uv2 = tempUV3;
                    uv3 = tempUV0;

                    break;

                case OcclusionTileset.TileRotation.down:
                    uv0 = tempUV2;
                    uv1 = tempUV3;
                    uv2 = tempUV0;
                    uv3 = tempUV1;

                    break;

                case OcclusionTileset.TileRotation.left:
                    uv0 = tempUV3;
                    uv1 = tempUV0;
                    uv2 = tempUV1;
                    uv3 = tempUV2;

                    break;
            }


            var flipUV0 = uv0;
            var flipUV1 = uv1;
            var flipUV2 = uv2;
            var flipUV3 = uv3;

            if (flipX)
            {
                uv0 = flipUV1;
                uv1 = flipUV0;
                uv2 = flipUV3;
                uv3 = flipUV2;
            }

            if (flipY)
            {
                uv0 = flipUV3;
                uv1 = flipUV2;
                uv2 = flipUV1;
                uv3 = flipUV0;
            }

            flipUV0 = uv0;
            flipUV1 = uv1;
            flipUV2 = uv2;
            flipUV3 = uv3;


            uv0 = flipUV3;
            uv1 = flipUV2;
            uv2 = flipUV1;
            uv3 = flipUV0;


            vertices.Add(new Vector3(tilePosition.x, tilePosition.y));
            vertices.Add(new Vector3(tilePosition.x + 1, tilePosition.y));
            vertices.Add(new Vector3(tilePosition.x + 1, tilePosition.y + 1));
            vertices.Add(new Vector3(tilePosition.x, tilePosition.y + 1));

            uv.Add(uv0);
            uv.Add(uv1);
            uv.Add(uv2);
            uv.Add(uv3);

            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);

            triangles.Add(0 + tileCount * 4);
            triangles.Add(1 + tileCount * 4);
            triangles.Add(2 + tileCount * 4);
            triangles.Add(2 + tileCount * 4);
            triangles.Add(3 + tileCount * 4);
            triangles.Add(0 + tileCount * 4);

            tileCount += 1;
        }

        public Mesh Export()
        {
            if (mesh == null) mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();

            // mesh.RecalculateNormals();

            return mesh;
        }
    }
}