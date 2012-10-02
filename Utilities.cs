using System;
using UnityEngine;

namespace MajiirKerbalLib
{
    static class Utilities
    {
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

        /// <summary>
        /// Adjusts a given setpoint so that rotation around the given axis is minimized.
        /// See: http://kerbalspaceprogram.com/forum/showthread.php/9398-One-Attitude-Controller-to-Rule-Them-All?p=149405&viewfull=1#post149405
        /// </summary>
        /// <param name="setpoint">Unadjusted setpoint</param>
        /// <param name="shipRotation">Current orientation of the vessel</param>
        /// <param name="axis">Axis about which to minimize rotation</param>
        /// <returns>Adjusted setpoint</returns>
        public static Quaternion UnrollAxis(Quaternion setpoint, Quaternion shipRotation, Vector3 axis)
        {
            var a = Quaternion.Dot(shipRotation, setpoint * new Quaternion(axis.x, axis.y, axis.z, 0));
            var q_qs = Quaternion.Dot(shipRotation, setpoint);
            var b = (a == 0) ? Math.Sign(q_qs) : (q_qs / a);
            var g = b / Mathf.Sqrt((b * b) + 1);
            var gu = Mathf.Sqrt(1 - (g * g)) * axis;
            var q_d = new Quaternion()
            {
                w = g,
                x = gu.x,
                y = gu.y,
                z = gu.z
            };
            return setpoint * q_d;
        }
    }
}
