using Code.Entities;
using Code.Extension;
using Code.FSM;
using Code.Managers;
using UnityEngine;

namespace Code.Towers.FSM
{
    public abstract class TowerState : EntityState
    {
        protected Tower _tower;

        public TowerState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _tower = entity as Tower;
        }
    }
}