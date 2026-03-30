using System;
using FunkyCode.Rendering.Light;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public class EdgePass
    {
        public float coreSize;

        public Vector2 edgeMiddle;
        public Vector2 edgePosition;
        public float edgeRotation;
        public float edgeSize;
        public Vector2 leftCoreIn;
        public Vector2 leftCoreInToEdge;
        public Vector2 leftCoreOut;
        public Vector2 leftCoreOutToEdge;
        public Vector2 leftEdge;

        public Vector2 leftEdgeLocal;

        public float leftInnerCore;

        public float leftInnerToEdge;

        public float leftOuterCore;

        public float leftOuterToEdge;
        public Vector2 projectedMiddle;
        public Vector2 rightCoreIn;
        public Vector2 rightCoreInToEdge;
        public Vector2 rightCoreOut;
        public Vector2 rightCoreOutToEdge;
        public Vector2 rightEdge;
        public Vector2 rightEdgeLocal;

        public float rightInnerCore;

        public float rightInnerToEdge;

        public float rightOuterCore;

        public float rightOuterToEdge;

        public float shadowTranslucency;

        public float var_1;
        public float var_2;
        public float var_3;
        public float var_4;
        public float var_5;
        public float var_6;
        public float var_7;

        public void SetVars()
        {
            var_1 = edgePosition.x;
            var_2 = edgePosition.y;
            var_3 = edgeRotation;
            var_4 = edgeSize;
            var_5 = ShadowEngine.drawOffset.x;
            var_6 = ShadowEngine.drawOffset.y;
            var_7 = shadowTranslucency;
            // + shadow depth?
        }

        public void Generate()
        {
            float lightSize = 1000;

            var dirX = Mathf.Cos(edgeRotation) * edgeSize;
            var dirY = Mathf.Sin(edgeRotation) * edgeSize;

            leftEdgeLocal = new Vector2(dirX, dirY);
            rightEdgeLocal = new Vector2(-dirX, -dirY);

            leftEdge = leftEdgeLocal + edgePosition;
            rightEdge = rightEdgeLocal + edgePosition;

            // left outer
            leftOuterCore = Mathf.Atan2(leftEdge.y, leftEdge.x) + Mathf.PI / 2;
            ;
            leftCoreOut.x = Mathf.Cos(leftOuterCore) * coreSize;
            leftCoreOut.y = Mathf.Sin(leftOuterCore) * coreSize;
            leftCoreOut.x = 0;
            leftCoreOut.y = 0;

            leftOuterToEdge = Mathf.Atan2(leftEdge.y - leftCoreOut.y, leftEdge.x - leftCoreOut.x);
            leftCoreOutToEdge = leftCoreOut; // middle
            leftCoreOutToEdge.x += Mathf.Cos(leftOuterToEdge) * lightSize;
            leftCoreOutToEdge.y += Mathf.Sin(leftOuterToEdge) * lightSize;

            leftInnerCore = Mathf.Atan2(leftEdge.y, leftEdge.x) - Mathf.PI / 2;
            ;
            leftCoreIn.x = Mathf.Cos(leftInnerCore) * coreSize;
            leftCoreIn.y = Mathf.Sin(leftInnerCore) * coreSize;

            leftInnerToEdge = Mathf.Atan2(leftEdge.y - leftCoreIn.y, leftEdge.x - leftCoreIn.x);
            leftCoreInToEdge = leftCoreIn; // middle
            leftCoreInToEdge.x += Mathf.Cos(leftInnerToEdge) * lightSize;
            leftCoreInToEdge.y += Mathf.Sin(leftInnerToEdge) * lightSize;

            // right outer
            rightOuterCore = Mathf.Atan2(rightEdge.y, rightEdge.x) + Mathf.PI / 2;
            ;
            rightCoreOut.x = Mathf.Cos(rightOuterCore) * coreSize;
            rightCoreOut.y = Mathf.Sin(rightOuterCore) * coreSize;

            rightOuterToEdge = Mathf.Atan2(rightEdge.y - rightCoreOut.y, rightEdge.x - rightCoreOut.x);
            rightCoreOutToEdge = rightCoreOut; // middle
            rightCoreOutToEdge.x += Mathf.Cos(rightOuterToEdge) * lightSize;
            rightCoreOutToEdge.y += Mathf.Sin(rightOuterToEdge) * lightSize;

            rightInnerCore = Mathf.Atan2(rightEdge.y, rightEdge.x) - Mathf.PI / 2;
            ;
            rightCoreIn.x = Mathf.Cos(rightInnerCore) * coreSize;
            rightCoreIn.y = Mathf.Sin(rightInnerCore) * coreSize;
            rightCoreIn.x = 0;
            rightCoreIn.y = 0;


            rightInnerToEdge = Mathf.Atan2(rightEdge.y - rightCoreIn.y, rightEdge.x - rightCoreIn.x);
            rightCoreInToEdge = rightCoreIn; // middle
            rightCoreInToEdge.x += Mathf.Cos(rightInnerToEdge) * lightSize;
            rightCoreInToEdge.y += Mathf.Sin(rightInnerToEdge) * lightSize;


            edgeMiddle = (leftEdge + rightEdge) / 2;

            var closestPoint = Math2D.ClosestPointOnLine(Vector2.zero, leftCoreOutToEdge, rightCoreInToEdge);
            var rotM = (float)Math.Atan2(closestPoint.y, closestPoint.x);
            projectedMiddle.x = (leftEdge.x + rightEdge.x) / 2 + Mathf.Cos(rotM) * lightSize;
            projectedMiddle.y = (leftEdge.y + rightEdge.y) / 2 + Mathf.Sin(rotM) * lightSize;
        }

        public void Draw()
        {
            GL.Color(new Color(var_4, var_5, var_6, var_7));
            GL.TexCoord3(var_1, var_2, var_3);

            var edgeAWorld = leftEdge;
            var edgeBWorld = rightEdge;

            var edgeALocal = leftEdgeLocal;
            var edgeBLocal = rightEdgeLocal;

            var lightDirection =
                (float)Math.Atan2((edgeAWorld.y + edgeBWorld.y) / 2, (edgeAWorld.x + edgeBWorld.x) / 2) * Mathf.Rad2Deg;
            var EdgeDirection =
                (float)Math.Atan2(edgeALocal.y - edgeBLocal.y, edgeALocal.x - edgeBLocal.x) * Mathf.Rad2Deg - 180;

            lightDirection -= EdgeDirection;
            lightDirection = (lightDirection + 720) % 360;


            if (lightDirection > 180)
            {
                GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                GL.Vertex3(edgeMiddle.x, edgeMiddle.y, 0);
                GL.Vertex3(leftEdge.x, leftEdge.y, 0);

                GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                GL.Vertex3(edgeMiddle.x, edgeMiddle.y, 0);
                GL.Vertex3(rightEdge.x, rightEdge.y, 0);
            }

            var fullResult =
                Math2D.GetPointLineIntersectLine3(leftEdge, leftCoreOutToEdge, rightEdge, rightCoreInToEdge);

            if (fullResult != null)
            {
                GL.Vertex3(fullResult.Value.x, fullResult.Value.y, 0);
                GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
            }
            else
            {
                var leftResult =
                    Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, rightEdge, rightCoreInToEdge);
                if (leftResult != null)
                {
                    GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                    GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                    GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);

                    GL.Vertex3(rightEdge.x, rightEdge.y, 0);
                    GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                    GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);

                    GL.Vertex3(rightEdge.x, rightEdge.y, 0);
                    GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);
                    GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);

                    GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                    GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                    GL.Vertex3(leftResult.Value.x, leftResult.Value.y, 0);
                }
                else
                {
                    var rightResult =
                        Math2D.GetPointLineIntersectLine3(edgeMiddle, projectedMiddle, leftEdge, leftCoreOutToEdge);

                    if (rightResult != null)
                    {
                        GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                        GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                        GL.Vertex3(rightEdge.x, rightEdge.y, 0);

                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                        GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                        GL.Vertex3(leftEdge.x, leftEdge.y, 0);

                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                        GL.Vertex3(rightResult.Value.x, rightResult.Value.y, 0);
                        GL.Vertex3(leftEdge.x, leftEdge.y, 0);

                        GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                        GL.Vertex3(rightResult.Value.x, rightResult.Value.y, 0);
                        GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                    }
                    else
                    {
                        if (lightDirection > 180)
                        {
                            GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                            GL.Vertex3(rightCoreInToEdge.x, rightCoreInToEdge.y, 0);
                            GL.Vertex3(rightEdge.x, rightEdge.y, 0);

                            GL.Vertex3(projectedMiddle.x, projectedMiddle.y, 0);
                            GL.Vertex3(leftCoreOutToEdge.x, leftCoreOutToEdge.y, 0);
                            GL.Vertex3(leftEdge.x, leftEdge.y, 0);
                        }
                    }
                }
            }
        }
    }
}