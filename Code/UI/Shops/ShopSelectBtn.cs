using Code.Core;
using Code.EventSystems;
using Code.References.Text;
using TMPro;
using UnityEngine;

namespace Code.UI.Shops
{
    public class ShopSelectBtn : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private GameEventChannelSO uiChannel;
        [SerializeField] private TextContainerSO textContainer;
        
        private SpawnableType _contentEnum;

        public void Initialize(SpawnableType contentEnum)
        {
            _contentEnum = contentEnum;
            string enumString = contentEnum.ToString();
            gameObject.name = $"{enumString}_Btn";

            contentText.text = textContainer.GetTextData(enumString).text[0];
        }

        public void OnClick()
        {
            uiChannel.RaiseEvent(UIEvents.ShopContentSelectEvent.Initializer(_contentEnum));
        }
    }
}