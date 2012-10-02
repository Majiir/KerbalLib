using UnityEngine;

namespace MajiirKerbalLib
{
    public class MajiirAdvancedSAS : global::Part
    {
        private Quaternion setpoint;
        private bool setpointSet = false;

        protected override void onFlightStart()
        {
            FlightInputHandler.OnFlyByWire += new FlightInputHandler.FlightInputCallback(fly);
        }

        protected override void onPartDestroy()
        {
            FlightInputHandler.OnFlyByWire -= new FlightInputHandler.FlightInputCallback(fly);
        }

        protected override void onDisconnect()
        {
            FlightInputHandler.OnFlyByWire -= new FlightInputHandler.FlightInputCallback(fly);
        }

        private void fly(FlightCtrlState s)
        {
            if (s.killRot)
            {
                if (!setpointSet)
                {
                    setpoint = vessel.transform.rotation;
                    setpointSet = true;
                }

                var rot = vessel.transform.rotation;

                var adjust = new Vector3(s.pitch, s.roll, s.yaw) * 0.05f;
                var adjustQ = new Quaternion(adjust.x, adjust.y, adjust.z, 1 - adjust.magnitude);

                setpoint = adjustQ * setpoint;

                if (Quaternion.Dot(setpoint, rot) < 0)
                {
                    setpoint.w *= -1;
                }

                var err = Quaternion.Inverse(setpoint) * rot;

                var control = vessel.angularVelocity + new Vector3(err.x, err.y, err.z);

                control.x = Mathf.Clamp(control.x, -1, 1);
                control.y = Mathf.Clamp(control.y, -1, 1);
                control.z = Mathf.Clamp(control.z, -1, 1);

                s.pitch = control.x;
                s.roll = control.y;
                s.yaw = control.z;
            }
            else
            {
                setpointSet = false;
            }
        }
    }
}
