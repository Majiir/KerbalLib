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
    }
}
