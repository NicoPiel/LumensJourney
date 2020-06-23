using System;
using System.Collections.Generic;

namespace Assets.SaveSystem
{
    [Serializable]
    public class Save
    {
        public int LightShardSave { get; set; }
        public int BankedLightShardsSave { get; set; }
        public Dictionary<string,  Dictionary<string, bool>> FlagsSave { get; set; }
        public int StoryStoneProgressionSave { get; set; }
        public int DiaryProgressionSave { get; set; }
        public int RunsCompletedSave { get; set; }
    }
}
