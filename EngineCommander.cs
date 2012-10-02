using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MajiirKerbalLib
{
    class EngineCommander
    {
        #region Static factory

        private static Dictionary<WeakReference<Vessel>, EngineCommander> commanders = new Dictionary<WeakReference<Vessel>, EngineCommander>();

        public static bool IsActive { get { return isActive; } set { isActive = value; } }
        private static bool isActive = true;

        public static EngineCommander GetInstance(Vessel vessel)
        {
            foreach (var wr in commanders.Keys.ToArray())
            {
                var v = wr.Target;
                if (v == null)
                {
                    commanders.Remove(wr);
                    MonoBehaviour.print(String.Format("[MajiirKerbalLib] Removed EngineCommander for collected vessel ({0} remaining)", commanders.Count));
                    continue;
                }
                if (v == vessel)
                {
                    return commanders[wr];
                }
            }

            var commander = new EngineCommander();
            commanders[new WeakReference<Vessel>(vessel)] = commander;
            MonoBehaviour.print(String.Format("[MajiirKerbalLib] Created EngineCommander for {0} ({1} total)", vessel.name, commanders.Count));
            return commander;
        }

        #endregion

        #region Static utility methods

        public static float UpdateThrust(float mainThrottle, LiquidFuelEngine engine)
        {
            if (!IsActive)
            {
                return mainThrottle;
            }
            if (engine.vessel == null)
            {
                MonoBehaviour.print(String.Format("[MajiirKerbalLib] Null vessel for {0}", engine.name));
                return mainThrottle;
            }
            return EngineCommander.GetInstance(engine.vessel).Update(mainThrottle, engine);
        }

        #endregion

        private int lastFrame = -1;
        private Dictionary<LiquidFuelEngine, float> throttleValues;

        private EngineCommander() { }

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
