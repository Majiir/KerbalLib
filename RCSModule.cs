using UnityEngine;

namespace MajiirKerbalLib
{
    public class RCSModule : global::RCSModule
    {
        protected override void onPartFixedUpdate()
        {
            if (this.vessel.rootPart.RequestRCS(0, 0))
            {
                this.stackIcon.SetIconColor(XKCDColors.White);
            }
            else
            {
                this.stackIcon.SetIconColor(XKCDColors.SlateGrey);
                base.onCtrlUpd(new FlightCtrlState());
            }
            base.onPartFixedUpdate();
        }
    }
}
