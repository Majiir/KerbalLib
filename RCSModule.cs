using UnityEngine;

namespace MajiirKerbalLib
{
    public class RCSModule : global::RCSModule
    {
        protected override void onPartFixedUpdate()
        {
            var commander = VesselCommander.GetInstance(this.vessel);
            if (this.vessel.rootPart.RequestRCS(commander.RequestedRCS, 0))
            {
                this.stackIcon.SetIconColor(XKCDColors.White);
            }
            else
            {
                this.stackIcon.SetIconColor(XKCDColors.SlateGrey);
                base.onCtrlUpd(new FlightCtrlState());
            }
            commander.RequestedRCS = 0;
            commander.ReturnRealRCS = false;
            base.onPartFixedUpdate();
            commander.ReturnRealRCS = true;
        }
    }
}
