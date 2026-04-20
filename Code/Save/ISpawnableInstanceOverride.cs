using Code.Core;

namespace Code.Save
{
    public interface ISpawnableInstanceOverride
    {
        public void SetData(SpawnableInstanceData instanceData);
        public SpawnableInstanceData GetInstanceData();
    }
}