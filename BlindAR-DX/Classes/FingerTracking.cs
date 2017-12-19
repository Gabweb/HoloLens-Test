
using System.Numerics;
using Windows.Graphics.Holographic;
using Windows.Perception.Spatial;
using Windows.UI.Input.Spatial;

namespace BlindAR_DX.Classes
{
    class FingerTracking
    {
        private SpatialInteractionManager interactionManager;
        private SpatialStationaryFrameOfReference referenceFrame;
        private HolographicSpace holographicSpace;

        public Vector3 position = new Vector3(0.0f, 0.0f, -2.0f);

        public FingerTracking(SpatialStationaryFrameOfReference referenceFrame, HolographicSpace holographicSpace)
        {
            this.referenceFrame = referenceFrame;
            this.holographicSpace = holographicSpace;

            interactionManager = SpatialInteractionManager.GetForCurrentView();
            interactionManager.SourceUpdated += this.sourceUpdate;
        }

        private void sourceUpdate(SpatialInteractionManager manager, SpatialInteractionSourceEventArgs args)
        {

            SpatialCoordinateSystem currentCoordinateSystem = referenceFrame.CoordinateSystem;
            SpatialInteractionSourceLocation pos = args.State.Properties.TryGetLocation(currentCoordinateSystem);

            HolographicFrame holographicFrame = holographicSpace.CreateNextFrame();

            // Get a prediction of where holographic cameras will be when this frame
            // is presented.
            HolographicFramePrediction prediction = holographicFrame.CurrentPrediction;
            // Get the gaze direction relative to the given coordinate system.
            Vector3 headPosition = (Vector3)pos.Position;
            SpatialPointerPose pose = SpatialPointerPose.TryGetAtTimestamp(currentCoordinateSystem, prediction.Timestamp);

            SpatialInteractionSource source = args.State.Source;

            Vector3 headDirection = pose.Head.ForwardDirection;

            // The hologram is positioned two meters along the user's gaze direction.
            float distanceFromUser = 0.1f; // meters
            Vector3 gazeAtTwoMeters = headPosition + (distanceFromUser * headDirection);

            // This will be used as the translation component of the hologram's
            // model transform.
            this.position = gazeAtTwoMeters;
        }

    }
}
