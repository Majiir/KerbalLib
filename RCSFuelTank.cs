using UnityEngine;

namespace MajiirKerbalLib
{
    public class RCSFuelTank : global::RCSFuelTank
    {
        private bool allowFlow = true;

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
            }
            if (this.state == PartStates.ACTIVE)
            {
                this.stackIcon.SetIconColor(XKCDColors.LightPeriwinkle);
            }
            base.onPartFixedUpdate();
        }

        public override bool RequestRCS(float amount, int earliestStage)
        {
            var commander = VesselCommander.GetInstance(this.vessel);
            if (!commander.ReturnRealRCS)
            {
                commander.RequestedRCS += amount;
                return true;
            }
            if (!allowFlow)
            {
                return false;
            }
            return base.RequestRCS(amount, earliestStage);
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
