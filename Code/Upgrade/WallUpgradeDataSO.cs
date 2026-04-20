using UnityEngine;

namespace Code.Upgrade
{
    [CreateAssetMenu(fileName = "WallUpgradeData", menuName = "SO/Upgrade/WallUpgradeData", order = 0)]
    public class WallUpgradeDataSO : UpgradeDataSO
    {
        public Mesh connectMesh;
    }
}