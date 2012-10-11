
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
                return this.EngineEnabled ? this.maximumEnginePower * this.totalEfficiency : 0;
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

        [KSPField(guiActive = true, guiName = "Specific Impulse", guiUnits = "s", guiFormat = "F1", isPersistant = false)]
        private float realIsp;

        protected override void onPartFixedUpdate()
        {
            base.onPartFixedUpdate();
            var commander = VesselCommander.GetInstance(this.vessel).EngineCommander;
            Events["DisableCommander"].active = commander.IsActive;
            Events["EnableCommander"].active = !commander.IsActive;
            realIsp = RealIsp;
        }

        [KSPEvent(guiActive = true, guiName = "Enable Command", active = false)]
        public void EnableCommander()
        {
            Events["DisableCommander"].active = true;
            Events["EnableCommander"].active = false;
            VesselCommander.GetInstance(this.vessel).EngineCommander.IsActive = true;
        }

        [KSPEvent(guiActive = true, guiName = "Disable Command")]
        public void DisableCommander()
        {
            Events["DisableCommander"].active = false;
            Events["EnableCommander"].active = true;
            VesselCommander.GetInstance(this.vessel).EngineCommander.IsActive = false;
        }
    }
}
