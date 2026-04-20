using System.Collections.Generic;

namespace Code.Core
{
    public interface ISpawnableSavable
    {
        public SpawnableInfoDatas GetSaveData();
        public void RestoreData(List<SpawnableInfoDatas> saveDatas);
    }
}