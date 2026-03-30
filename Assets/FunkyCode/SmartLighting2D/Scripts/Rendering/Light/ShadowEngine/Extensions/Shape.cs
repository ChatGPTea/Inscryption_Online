namespace FunkyCode.Rendering.Light.Shadow
{
    public class Shape
    {
        public static void Draw(Light2D light, LightCollider2D lightCollider)
        {
            if (!lightCollider.InLight(light)) return;

            if (light.eventPresetId > 0)
                // optimize - only if event handling enabled
                // used to update light when light collider leaves light bounds
                light.AddCollider(lightCollider);

            var shadowMin = lightCollider.shadowDistanceMin;
            var shadowMax = lightCollider.shadowDistanceMax;

            if (lightCollider.shadowDistance == LightCollider2D.ShadowDistance.Infinite)
            {
                shadowMin = 0;
                shadowMax = 0;
            }

            var shape = lightCollider.mainShape;

            var polygons = shape.GetPolygonsWorld();

            ShadowEngine.Draw(polygons, shadowMin, shadowMax, lightCollider.shadowTranslucency);
        }
    }
}