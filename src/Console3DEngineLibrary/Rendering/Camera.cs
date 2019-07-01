namespace Console3DEngineLibrary.Rendering
{
    internal class Camera
    {
        internal Vector CameraPlaneVector;
        internal Vector DirectionVector;
        internal Vector PositionVector;

        public Camera(double fov)
        {
            DirectionVector = new Vector {X = -1, Y = 0};
            CameraPlaneVector = new Vector {X = 0, Y = fov / 100};
        }
    }
}