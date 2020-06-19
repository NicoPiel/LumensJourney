using System;
using System.Collections.Generic;

namespace Assets.SaveSystem
{
    [Serializable]
    public class Save
    {
        public int LightShard { get; set; }
        public int SmithProgress { get; set; }
        public int BankedLightShards { get; set; }
        public Dictionary<string,  Dictionary<string, bool>> Flags { get; set; }
    }
}
