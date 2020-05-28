namespace Assets.SaveSystem
{
    [System.Serializable]
    public class Save
    {
        public int LightShards {get; set;}
        public int SmithProgress {get; set;}
    
        public Save()
        {
            LightShards = 0;
            SmithProgress = 0;
        }
    }
}
