using Code.EventSystems;
using UnityEngine;

namespace Code.UI.Stages
{
    public class CombatStageBtn : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiChannel;
        
        public void PopUpUI()
        {
            uiChannel.RaiseEvent(UIEvents.ToggleStageChoiceUI);
        }
    }
}