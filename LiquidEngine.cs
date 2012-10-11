
namespace MajiirKerbalLib
{
    public class LiquidEngine : global::LiquidEngine, IEngine
    {
        [KSPField]
        public bool EngineEnabled
        {
            get { return engineEnabled; }
            private set
            {
                if (engineEnabled && !value)
                {
                    realMaxThrust = maxThrust;
                    realMinThrust = minThrust;
                    maxThrust = minThrust = 0;
                }
                if (!engineEnabled && value)
                {
                    maxThrust = realMaxThrust;
                    minThrust = realMinThrust;
                }
                engineEnabled = value;
            }
        }
        private bool engineEnabled = true;

        private float realMaxThrust;
        private float realMinThrust;

        public float MaxThrust
        {
            get { return this.maxThrust; }
        }

        protected override void onCtrlUpd(FlightCtrlState s)
        {
            var state = Utilities.CopyFlightCtrlState(s);
            state.mainThrottle = EngineEnabled ? EngineCommander.UpdateThrust(state.mainThrottle, this) : 0;
            if (state.mainThrottle == 0)
            {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd(state);
        }
        
        public float RealIsp
        {
            get
            {
                var massFlowrate = (this.fuelConsumption / Utilities.FuelDensity);
                return this.MaxThrust / (massFlowrate * Utilities.SurfaceGravity);
            }
        }
        
        protected override void onActiveFixedUpdate()
        {
            var temp = this.temperature;
            base.onActiveFixedUpdate();
            if (!this.EngineEnabled)
            {
                this.temperature = temp;
            }
        }

        [KSPEvent(guiActive = true, guiName = "Activate", active = false)]
        public void EnableEngine()
        {
            Events["DisableEngine"].active = true;
            Events["EnableEngine"].active = false;
            EngineEnabled = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate")]
        public void DisableEngine()
        {
            Events["DisableEngine"].active = false;
            Events["EnableEngine"].active = true;
            EngineEnabled = false;
        }
    }
}
