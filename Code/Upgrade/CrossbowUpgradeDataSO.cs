using UnityEngine;

namespace Code.Upgrade
{
    [CreateAssetMenu(fileName = "CrossbowUpgradeData", menuName = "SO/Upgrade/CrossbowUpgradeData", order = 0)]
    public class CrossbowUpgradeDataSO : UpgradeDataSO
    {
        public int arrowCnt;
        public float arrowSpreadAngle;
    }
}