using Code.Core;
using Code.Scenes;
using Code.Stages;
using Code.UI.Combat;
using Code.Units;

namespace Code.EventSystems
{
    public static class UIEvents
    {
        public static readonly ShopToggleEvent ShopToggleEvent = new ShopToggleEvent();
        public static readonly ShopContentSelectEvent ShopContentSelectEvent = new ShopContentSelectEvent();
        public static readonly UnitAddEvent UnitAddEvent = new UnitAddEvent();
        public static readonly ExtractGoldEvent ExtractGoldEvent = new ExtractGoldEvent();
        public static readonly PopUpResultUIEvent PopUpResultUIEvent = new PopUpResultUIEvent();
        public static readonly StarConditionSendEvent StarConditionSendEvent = new StarConditionSendEvent();
        public static readonly ToggleStageChoiceUI ToggleStageChoiceUI = new ToggleStageChoiceUI();
        public static readonly PopUpStageEnterUI PopUpStageEnterUI = new PopUpStageEnterUI();
        public static readonly PopUpTransitionUI PopUpTransitionUI = new PopUpTransitionUI();
        public static readonly UseCostEvent UseCostEvent = new UseCostEvent();
        public static readonly NameRegisterEvent NameRegisterEvent = new NameRegisterEvent();
        public static readonly PopUpSellCheckUIEvent PopUpSellCheckUIEvent = new PopUpSellCheckUIEvent();
    }

    public class ShopToggleEvent : GameEvent
    {
        public bool isActive;

        public ShopToggleEvent Initializer(bool isActive)
        {
            this.isActive = isActive;
            return this;
        }
    }

    public class ShopContentSelectEvent : GameEvent
    {
        public SpawnableType contentEnum;

        public ShopContentSelectEvent Initializer(SpawnableType contentEnum)
        {
            this.contentEnum = contentEnum;
            return this;
        }
    }

    public class UnitAddEvent : GameEvent
    {
        public UnitDataSO unitData;

        public UnitAddEvent Initializer(UnitDataSO unitData)
        {
            this.unitData = unitData;
            return this;
        }
    }

    public class ExtractGoldEvent : GameEvent
    {
    }

    public class PopUpResultUIEvent : GameEvent
    {
        public bool isClear;

        public PopUpResultUIEvent Initializer(bool isClear)
        {
            this.isClear = isClear;

            return this;
        }
    }

    public class StarConditionSendEvent : GameEvent
    {
        public StarCondition condition;

        public StarConditionSendEvent Initializer(StarCondition condition)
        {
            this.condition = condition;
            return this;
        }
    }

    public class ToggleStageChoiceUI : GameEvent
    {
    }

    public class PopUpStageEnterUI : GameEvent
    {
        public StageDataSO stageData;
        public bool isFirstClear;
        public bool isActive;

        public PopUpStageEnterUI Initializer(StageDataSO stageData, bool isFirstClear, bool isActive)
        {
            this.stageData = stageData;
            this.isFirstClear = isFirstClear;
            this.isActive = isActive;
            return this;
        }
    }

    public class PopUpTransitionUI : GameEvent
    {
        public bool isFadeIn;
        public SceneDataSO nextScene;

        public PopUpTransitionUI Initializer(bool isFadeIn, SceneDataSO nextScene)
        {
            this.isFadeIn = isFadeIn;
            this.nextScene = nextScene;
            return this;
        }
    }

    public class UseCostEvent : GameEvent
    {
        public int cost;

        public UseCostEvent Initializer(int cost)
        {
            this.cost = cost;
            return this;
        }
    }

    public class NameRegisterEvent : GameEvent
    {
        public string userName;

        public NameRegisterEvent Initializer(string userName)
        {
            this.userName = userName;
            return this;
        }
    }

    public class PopUpSellCheckUIEvent : GameEvent
    {
        public ISpawnable spawnable;
        public IUpgradeable upgradeable;

        public PopUpSellCheckUIEvent Initializer(ISpawnable spawnable, IUpgradeable upgradeable)
        {
            this.spawnable = spawnable;
            this.upgradeable = upgradeable;
            return this;
        }
    }
}