using UnityEngine;

namespace MajiirKerbalLib
{
    public class FuelTank : global::FuelTank
    {
        protected override void onPartStart()
        {
            this.started = true;
            base.onPartStart();
        }

        protected override void onPartFixedUpdate()
        {
            if ((this.state == PartStates.DEACTIVATED) && (this.fuel > 0))
            {
                this.state = PartStates.ACTIVE;
                this.stackIcon.SetIconColor(XKCDColors.BrightTeal);
                this.DrainFuel(0);
            }
            base.onPartFixedUpdate();
            var commander = VesselCommander.GetInstance(this.vessel).EngineCommander;
            Events["DisableFlow"].active = this.allowFlow;
            Events["EnableFlow"].active = !this.allowFlow;
        }

        [KSPEvent(guiActive = true, guiName = "Enable flow", active = false)]
        public void EnableFlow()
        {
            Events["DisableFlow"].active = true;
            Events["EnableFlow"].active = false;
            this.allowFlow = true;
        }

        [KSPEvent(guiActive = true, guiName = "Disable flow")]
        public void DisableFlow()
        {
            Events["DisableFlow"].active = false;
            Events["EnableFlow"].active = true;
            this.allowFlow = false;
        }
    }
}
