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
        }

        [KSPEvent(guiActive = false, guiName = "Enable flow")]
        private void AllowFlow()
        {
            this.allowFlow = true;
            UpdateGui();
        }

        [KSPEvent(guiActive = true, guiName = "Disable flow")]
        private void DenyFlow()
        {
            this.allowFlow = false;
            UpdateGui();
        }

        private void UpdateGui()
        {
            Events["AllowFlow"].guiActive = Events["AllowFlow"].active = !this.allowFlow;
            Events["DenyFlow"].guiActive = Events["DenyFlow"].active = this.allowFlow;
        }
    }
}
