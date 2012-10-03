using UnityEngine;

namespace MajiirKerbalLib
{
    public class RCSFuelTank : global::RCSFuelTank
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
                    this.stackIcon.SetIconColor(XKCDColors.LightPeriwinkle);
                }
                else
                {
                    MonoBehaviour.print("[MajiirKerbalLib] StackIcon was null!");
                }
                this.getFuel(0);
            }
            base.onPartFixedUpdate();
        }
    }
}
