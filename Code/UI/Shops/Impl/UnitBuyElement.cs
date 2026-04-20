using Code.EventSystems;
using Code.Units;
using DG.Tweening;
using UnityEngine;

namespace Code.UI.Shops.Impl
{
    public class UnitBuyElement : SpawnableBuyElement<Unit>
    {
        protected override void PlayBuyTween(bool canBuy)
        {
            _buyTween?.Complete();

            if (canBuy)
            {
                _spawnManager.Create(_spawnableData);
                _buyTween = btnImage.DOColor(Color.green, buyTweenDuration).SetUpdate(true).SetLoops(2, LoopType.Yoyo);
            }
            else
            {
                _buyTween = btnImage.DOColor(Color.red, buyTweenDuration).SetUpdate(true).SetLoops(2, LoopType.Yoyo);
            }
        }
    }
}