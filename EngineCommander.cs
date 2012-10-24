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

        public static float UpdateDifferentialThrust<T>(float mainThrottle, Vector3 torqueCommand, T engine) where T : global::Part, IEngine
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
            return commander.UpdateDifferential(mainThrottle, torqueCommand, engine);
        }

        #endregion

        public bool IsActive { get; set; }

        private int lastFrame = -1;
        private Dictionary<IEngine, float> throttleValues;

        public EngineCommander()
        {
            IsActive = true;
        }

        private float UpdateDifferential<T>(float mainThrottle, Vector3 torqueCommand, T targetEngine) where T : global::Part, IEngine
        {
            if (targetEngine.State != PartStates.ACTIVE) { return mainThrottle; }
            var vessel = targetEngine.vessel;

            if (Time.frameCount != lastFrame)
            {                
                lastFrame = Time.frameCount;

                var com = vessel.findWorldCenterOfMass();
                var up = vessel.transform.up;

                torqueCommand = vessel.transform.TransformDirection(torqueCommand);

                var torques = new Dictionary<IEngine, Vector3>();

                foreach (var engine in vessel.parts.OfType<IEngine>())
                {
                    var part = (global::Part)engine;                    
                    var comO = com - part.transform.position;

                    var torque = comO.Cross(engine.MaxThrust * part.transform.up); // this is a FORCE vector, not the direction the thrust goes. duh.
                    torques.Add(engine, torque);
                }

                throttleValues = new Dictionary<IEngine, float>();
                var max = torques.Values.Select(v => v.magnitude).Max();

                foreach (var kvp in torques)
                {
                    var engine = kvp.Key;
                    var torque = kvp.Value;
                    torque = max > 0 ? torque / max : new Vector3();
                    throttleValues[engine] = Mathf.Clamp01(mainThrottle + torqueCommand.Dot(torque) * 0.25f);
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

        private float Update<T>(float mainThrottle, T targetEngine) where T : global::Part, IEngine
        {
            if (targetEngine.State != PartStates.ACTIVE) { return mainThrottle; }

            if (Time.frameCount != lastFrame)
            {
                lastFrame = Time.frameCount;

                var engineGroups = new SortedDictionary<float, List<IEngine>>();

                float thrust = 0;
                foreach (var part in targetEngine.vessel.parts)
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
