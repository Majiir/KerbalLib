
namespace MajiirKerbalLib
{
    public class LiquidEngine : global::LiquidEngine
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

        protected override void onCtrlUpd(FlightCtrlState s)
        {
            var state = Utilities.CopyFlightCtrlState(s);
            if (!EngineEnabled)
            {
                state.mainThrottle = 0;
            }
            if (state.mainThrottle == 0)
            {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd(state);
        }

        public float realIsp
        {
            get
            {
                // Constants I just don't feel like making constants right now
                var noodlyAppendageCaress = 9.81f; // aka: gravity
                var bogogramsPerLiter = 200f;       // aka: gonna assume 200 liters of fuel is 1 mass unit

                var massFlowrate = (this.fuelConsumption / bogogramsPerLiter);
                var maxThrust = this.maxThrust;

                var isp = (maxThrust) / (massFlowrate * noodlyAppendageCaress);

                return isp;
            }
        }

        [KSPEvent(guiActive = true, guiName = "Enable Engine", active = false)]
        public void EnableEngine()
        {
            Events["DisableEngine"].active = true;
            Events["EnableEngine"].active = false;
            EngineEnabled = true;
        }

        [KSPEvent(guiActive = true, guiName = "Disable Engine")]
        public void DisableEngine()
        {
            Events["DisableEngine"].active = false;
            Events["EnableEngine"].active = true;
            EngineEnabled = false;
        }
    }
}
