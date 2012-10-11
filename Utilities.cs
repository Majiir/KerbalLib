using System;
using UnityEngine;

namespace MajiirKerbalLib
{
    internal static class Utilities
    {
        public const float SurfaceGravity = 9.81f;
        public const float FuelDensity = 200f;

        public static FlightCtrlState CopyFlightCtrlState(FlightCtrlState state)
        {
            var s = new FlightCtrlState();
            s.activate = state.activate;
            s.fastThrottle = state.fastThrottle;
            s.gearDown = state.gearDown;
            s.gearUp = state.gearUp;
            s.headlight = state.headlight;
            s.killRot = state.killRot;
            s.mainThrottle = state.mainThrottle;
            s.pitch = state.pitch;
            s.roll = state.roll;
            s.X = state.X;
            s.Y = state.Y;
            s.yaw = state.yaw;
            s.Z = state.Z;
            return s;
        }
    }
}
