using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MajiirKerbalLib
{
    internal class EngineCommander
    {
        #region Static utility methods

        public static float UpdateThrust<T>(float mainThrottle, T engine) where T : global::Part, IEngine
        {
            if (engine.vessel == null)
            {
                MonoBehaviour.print(String.Format("[MajiirKerbalLib] Null vessel for {0}", engine.name));
                return mainThrottle;
            }
            var commander = VesselCommander.GetInstance(engine.vessel).EngineCommander;
            if (!commander.IsActive)
            {
                return mainThrottle;
            }
            return commander.Update(mainThrottle, engine);
        }

        #endregion

        public bool IsActive { get; set; }

        private int lastFrame = -1;
        private Dictionary<IEngine, float> throttleValues;

        public EngineCommander()
        {
            IsActive = true;
        }

        private float Update<T>(float mainThrottle, T targetEngine) where T : global::Part, IEngine
        {
            if (targetEngine.State != PartStates.ACTIVE) { return mainThrottle; }

            if (Time.frameCount != lastFrame)
            {
                lastFrame = Time.frameCount;
                ComputeThrottles(mainThrottle, targetEngine.vessel);
            }
            if (throttleValues.ContainsKey(targetEngine))
            {
                return throttleValues[targetEngine];
            }
            else
            {
                MonoBehaviour.print(String.Format("[MajiirKerbalLib] Couldn't find throttle level for {0}", targetEngine.name));
                return mainThrottle;
            }
        }

        private void ComputeThrottles(float mainThrottle, Vessel vessel)
        {
            var engines = vessel.parts.Where(p => p.State == PartStates.ACTIVE).OfType<IEngine>();
            var thrust = engines.Sum(e => e.MaxThrust);

            thrust *= mainThrottle;
            throttleValues = new Dictionary<IEngine, float>();

            foreach (var group in engines.ToLookup(e => e.RealIsp).OrderByDescending(g => g.Key))
            {
                var availableThrust = group.Sum(e => e.MaxThrust);
                var groupThrust = Math.Min(availableThrust, thrust);
                thrust -= groupThrust;
                var throttle = availableThrust > 0 ? groupThrust / availableThrust : 0;
                foreach (var engine in group)
                {
                    throttleValues[engine] = throttle;
                }
            }
        }
    }
}
