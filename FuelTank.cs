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
                if (this.stackIcon != null)
                {
                    this.stackIcon.SetIconColor(XKCDColors.BrightTeal);
                }
                else
                {
                    MonoBehaviour.print("[MajiirKerbalLib] StackIcon was null!");
                }
                this.DrainFuel(0);
            }
            base.onPartFixedUpdate();
        }
    }
}
