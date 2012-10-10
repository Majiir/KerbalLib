namespace MajiirKerbalLib
{
    public class AtmosphericEngine : global::AtmosphericEngine, IEngine
    {
        protected override void onCtrlUpd (FlightCtrlState s)
        {
            var state = Utilities.CopyFlightCtrlState (s);
            state.mainThrottle = this.engineEnabled ? EngineCommander.UpdateThrust(state.mainThrottle, this) : 0;
            if (state.mainThrottle == 0) {
                state.pitch = state.roll = state.yaw = 0;
            }
            base.onCtrlUpd (state);
        }
        
        public float MaxThrust {
            get {
                return this.maximumEnginePower * this.airflowEfficiency;
            }
        }
        
        public float RealIsp {
            get {
                // Constants I just don't feel like making constants right now
                var noodlyAppendageCaress = 9.81f; // aka: gravity
                var bogogramsPerLiter = 200f;       // aka: gonna assume 200 liters of fuel is 1 mass unit
                
                var massFlowrate = (this.fuelConsumption / bogogramsPerLiter);
                var maxThrust = this.MaxThrust;
                
                var isp = (maxThrust) / (massFlowrate * noodlyAppendageCaress);

                return isp;
            }
        }   

    }
}
