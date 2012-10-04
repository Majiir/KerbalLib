using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MajiirKerbalLib
{
    internal class VesselCommander
    {
        #region Static factory

        private static Dictionary<WeakReference<Vessel>, VesselCommander> commanders = new Dictionary<WeakReference<Vessel>, VesselCommander>();

        public static VesselCommander GetInstance(Vessel vessel)
        {
            foreach (var wr in commanders.Keys.ToArray())
            {
                var v = wr.Target;
                if (v == null)
                {
                    commanders.Remove(wr);
                    MonoBehaviour.print(String.Format("[MajiirKerbalLib] Removed VesselCommander for collected vessel ({0} remaining)", commanders.Count));
                    continue;
                }
                if (v == vessel)
                {
                    return commanders[wr];
                }
            }

            var commander = new VesselCommander();
            commanders[new WeakReference<Vessel>(vessel)] = commander;
            MonoBehaviour.print(String.Format("[MajiirKerbalLib] Created VesselCommander for {0} ({1} total)", vessel.name, commanders.Count));
            return commander;
        }

        #endregion

        private VesselCommander()
        {
            this.EngineCommander = new EngineCommander();
            this.ReturnRealRCS = true;
        }

        public EngineCommander EngineCommander { get; private set; }

        public bool ReturnRealRCS { get; set; }
        public float RequestedRCS { get; set; }
    }
}
