using TMPro;
using UnityEngine;

namespace Code.UI.Gold
{
    public class UpgradeGoldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldText;

        public void SetText(bool isEnable, int amount)
        {
            if(isEnable) goldText.text = amount.ToString();
            
            gameObject.SetActive(isEnable);
        }
    }
}