
namespace MajiirKerbalLib
{
    public class LiquidFuelEngine : global::LiquidFuelEngine
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
            state.mainThrottle = EngineEnabled ? EngineCommander.UpdateThrust(state.mainThrottle, this) : 0;
            if (state.mainThrottle == 0)
            {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd(state);
        }

        protected override void onPartFixedUpdate()
        {
            base.onPartFixedUpdate();
            var commander = VesselCommander.GetInstance(this.vessel).EngineCommander;
            Events["DisableCommander"].active = commander.IsActive;
            Events["EnableCommander"].active = !commander.IsActive;
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
