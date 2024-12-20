namespace rt
{
    class RayTracer
    {
        private Geometry[] geometries;
        private Light[] lights;

        public RayTracer(Geometry[] geometries, Light[] lights)
        {
            this.geometries = geometries;
            this.lights = lights;
        }

        private double ImageToViewPlane(int n, int imgSize, double viewPlaneSize)
        {
            return -n * viewPlaneSize / imgSize + viewPlaneSize / 2;
        }

        private Intersection FindFirstIntersection(Line ray, double minDist, double maxDist)
        {
            var intersection = Intersection.NONE;

            foreach (var geometry in geometries)
            {
                var intr = geometry.GetIntersection(ray, minDist, maxDist);

                if (!intr.Valid || !intr.Visible) continue;

                if (!intersection.Valid || !intersection.Visible)
                {
                    intersection = intr;
                }
                else if (intr.T < intersection.T)
                {
                    intersection = intr;
                }
            }

            return intersection;
        }

        private bool IsLit(Vector point, Light light)
        {
            // TODO: ADD CODE HERE
            var line = new Line(point, light.Position);
            var segmentLength = (point - light.Position).Length();
            //var isBlockedByGeometry= false;
            foreach (var geometry in geometries)
            {
                if (geometry is RawCtMask)
                {
                    continue;
                }
                var intersection = geometry.GetIntersection(line, 0.0001d, segmentLength);
                if (intersection.Visible)
                {
                    //isBlockedByGeometry = true;
                    return false;
                }
            }
            //return !isBlockedByGeometry;
            return true;
        }

        public void Render(Camera camera, int width, int height, string filename)
        {
            // TODO: ADD CODE HERE
            var background = new Color(0.2, 0.2, 0.2, 1.0);
            var viewParallel = (camera.Up ^ camera.Direction).Normalize();

            var image = new Image(width, height);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    
                    var x1 = camera.Position + camera.Direction * camera.ViewPlaneDistance + viewParallel * ImageToViewPlane(i, width, camera.ViewPlaneWidth) + camera.Up * ImageToViewPlane(j, height, camera.ViewPlaneHeight);
                    var ray = new Line(camera.Position, x1);
                    var intersect = FindFirstIntersection(ray, camera.FrontPlaneDistance, camera.BackPlaneDistance);

                    if (intersect.Valid && intersect.Visible)
                    {
                        var color = new Color();

                        var V = intersect.Position;
                        var E = (camera.Position - V).Normalize();
                        var N = intersect.Normal;

                        foreach (var light in lights)
                        {
                            color += intersect.Material.Ambient * light.Ambient;
                            if (IsLit(V, light))
                            {
                                var T = (light.Position - V).Normalize();
                                var R = (N * (N * T) * 2 - T).Normalize();
                                if (N * T > 0.001d)
                                {
                                    color += intersect.Material.Diffuse * light.Diffuse * (N * T);
                                }
                                if (E * R > 0.001d)
                                {
                                    color += intersect.Material.Specular * light.Specular * Math.Pow(E * R, intersect.Material.Shininess);
                                }
                                color *= light.Intensity;
                            }
                        }
                        image.SetPixel(i, j, color);
                    }
                    else
                    {
                        image.SetPixel(i, j, background);
                    }
                }
            }
            image.Store(filename);
        }
    }
}