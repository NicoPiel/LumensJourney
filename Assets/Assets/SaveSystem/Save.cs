using System;
using System.Collections.Generic;

namespace Assets.SaveSystem
{
    [Serializable]
    public class Save
    {
        public int lightShard;
        public int smithProgress;
        public int bankedLightShards;
        public Dictionary<string, Dictionary<string, bool>> flags;
    }
}
