using UnityModManagerNet;

namespace YentisMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Show NPC names on map")] public bool showNpcNames = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
}
