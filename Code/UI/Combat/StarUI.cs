using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.UI.Combat
{
    public class StarUI : MonoBehaviour
    {
        public UnityEvent OnStarSetted;
        
        [field: SerializeField] public StarCondition Condition { get; private set; }
        [SerializeField] private Image starImage;
        [SerializeField] private Sprite starSprite;
        [SerializeField] private float starSizeMultiplier = 1.5f;
        [SerializeField] private float starReduceDuration = 0.2f;
        [SerializeField] private float initDelay = 0.8f;

        private Vector2 _originScale;
        
        public void SetStar()
        {
            _originScale = starImage.transform.localScale;
            
            starImage.transform.localScale = Vector2.one * starSizeMultiplier;
            starImage.sprite = starSprite;

            DOVirtual.DelayedCall(initDelay, HandleInitEnd).SetUpdate(true);
        }

        private void HandleInitEnd()
        {
            starImage.transform.DOScale(_originScale, starReduceDuration).SetEase(Ease.InQuart).SetUpdate(true)
                .OnComplete(() => OnStarSetted?.Invoke());
        }
    }
}