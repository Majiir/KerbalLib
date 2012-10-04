using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MajiirKerbalLib
{
    internal class EngineCommander
    {
        #region Static utility methods

        public static float UpdateThrust(float mainThrottle, LiquidFuelEngine engine)
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
        private Dictionary<LiquidFuelEngine, float> throttleValues;

        public EngineCommander()
        {
            IsActive = true;
        }

        private float Update(float mainThrottle, LiquidFuelEngine targetEngine)
        {
            if (targetEngine.State != PartStates.ACTIVE) { return mainThrottle; }

            if (Time.frameCount != lastFrame)
            {
                lastFrame = Time.frameCount;

                var engineGroups = new SortedDictionary<float, List<LiquidFuelEngine>>();

                float thrust = 0;
                foreach (var part in targetEngine.vessel.parts)
                {
                    var engine = part as LiquidFuelEngine;
                    if (engine != null)
                    {
                        if (engine.State != PartStates.ACTIVE) { continue; }
                        thrust += engine.maxThrust;

                        if (!engineGroups.ContainsKey(engine.realIsp)) { engineGroups[engine.realIsp] = new List<LiquidFuelEngine>(); }
                        engineGroups[engine.realIsp].Add(engine);
                    }
                }

                thrust *= mainThrottle;
                throttleValues = new Dictionary<LiquidFuelEngine, float>();

                foreach (var kvp in engineGroups.Reverse())
                {
                    var group = kvp.Value;
                    float availableThrust = 0;
                    foreach (var engine in group)
                    {
                        availableThrust += engine.maxThrust;
                    }
                    var groupThrust = Math.Min(availableThrust, thrust);
                    thrust -= groupThrust;
                    var throttle = groupThrust / availableThrust;
                    foreach (var engine in group)
                    {
                        throttleValues[engine] = throttle;
                    }
                }
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
    }
}
