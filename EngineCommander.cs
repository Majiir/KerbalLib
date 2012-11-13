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
            var engineGroups = new SortedDictionary<float, List<IEngine>>();

            float thrust = 0;
            foreach (var part in vessel.parts)
            {
                var engine = part as IEngine;
                if (engine != null)
                {
                    if (part.State != PartStates.ACTIVE) { continue; }
                    thrust += engine.MaxThrust;

                    if (!engineGroups.ContainsKey(engine.RealIsp)) { engineGroups[engine.RealIsp] = new List<IEngine>(); }
                    engineGroups[engine.RealIsp].Add(engine);
                }
            }

            thrust *= mainThrottle;
            throttleValues = new Dictionary<IEngine, float>();

            foreach (var kvp in engineGroups.Reverse())
            {
                var group = kvp.Value;
                float availableThrust = 0;
                foreach (var engine in group)
                {
                    availableThrust += engine.MaxThrust;
                }
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
