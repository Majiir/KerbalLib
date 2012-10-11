
namespace MajiirKerbalLib
{
    public class AtmosphericEngine : global::AtmosphericEngine, IEngine
    {
        protected override void onCtrlUpd (FlightCtrlState s)
        {
            var state = Utilities.CopyFlightCtrlState (s);
            state.mainThrottle = this.engineEnabled ? EngineCommander.UpdateThrust(state.mainThrottle, this) : 0;
            if (state.mainThrottle == 0) {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd (state);
        }

        public bool EngineEnabled
        {
            get { return this.engineEnabled; }
        }

        public float MaxThrust
        {
            get
            {
                return this.maximumEnginePower * this.airflowEfficiency;
            }
        }

        public float RealIsp
        {
            get
            {
                var massFlowrate = (this.fuelConsumption / Utilities.FuelDensity);
                return this.MaxThrust / (massFlowrate * Utilities.SurfaceGravity);
            }
        }
    }
}
