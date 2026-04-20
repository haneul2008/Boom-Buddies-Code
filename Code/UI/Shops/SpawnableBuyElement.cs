using System;
using Code.Core;
using Code.Entities;
using Code.EventSystems;
using Code.Managers;
using DG.Tweening;
using HNLib.Dependencies;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.UI.Shops
{
    public abstract class SpawnableBuyElement<T> : MonoBehaviour where T : ISpawnable
    {
        [SerializeField] protected GameEventChannelSO uiChannel;
        [SerializeField] protected Image spawnableImage;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI goldText;
        [SerializeField] protected Image btnImage;
        [SerializeField] protected float buyTweenDuration;
        
        protected Tween _buyTween;
        protected SpawnableDataSO _spawnableData;
        protected SpawnManager<T> _spawnManager;
        protected GoldManager _goldManager;

        public virtual void Initialize(SpawnableDataSO data)
        {
            _spawnableData = data;
            spawnableImage.sprite = data.spawnableSprite;
            nameText.text = data.spawnableName;
            goldText.text = data.requiredGold.ToString();
            _spawnManager = CreateOnceManager.Instance.GetManager<SpawnManager<T>>();
            _goldManager = CreateOnceManager.Instance.GetManager<GoldManager>();
        }

        protected virtual void OnDestroy()
        {
            _buyTween?.Kill();
        }

        public virtual void BuyItem()
        {
            bool canBuy = _goldManager.CurrentGold >= _spawnableData.requiredGold;
            PlayBuyTween(canBuy);
        }

        protected virtual void PlayBuyTween(bool canBuy)
        {
            _buyTween?.Complete();

            if (canBuy)
            {
                uiChannel.RaiseEvent(UIEvents.ShopToggleEvent.Initializer(false));
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