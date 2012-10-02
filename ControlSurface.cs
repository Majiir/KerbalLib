
namespace MajiirKerbalLib
{
    public class ControlSurface : global::ControlSurface
    {
        protected override void onCtrlUpd(FlightCtrlState s)
        {
            var state = Utilities.CopyFlightCtrlState(s);
            if (this.staticPressureAtm <= double.Epsilon)
            {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd(state);
        }
    }
}
