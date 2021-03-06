﻿using Codebreak.Service.World.Game.Fight.Effect;
using Codebreak.Service.World.Game.Map;
using Codebreak.Service.World.Game.Spell;
using Codebreak.Service.World.Manager;
using Codebreak.Service.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Fight
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractActivableObject : IFightObstacle
    {
        /// <summary>
        /// 
        /// </summary>
        public FightObstacleTypeEnum ObstacleType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Priority
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public ActiveType ActivationType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Color
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanGoThrough
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanStack
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Duration
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Hide
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ActionId
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<FightCell> AffectedCells
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Activated
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public FightCell Cell
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AbstractFighter> Targets
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        protected AbstractFight m_fight;
        protected AbstractFighter m_caster;
        protected SpellTemplate m_actionSpell;
        protected SpellLevel m_actionEffect;
        protected int m_spellId;

        /// <summary>
        /// 
        /// </summary>
        public abstract void AppearForAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        public abstract void Appear(MessageDispatcher dispatcher);

        /// <summary>
        /// 
        /// </summary>
        public abstract void DisappearForAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="activeType"></param>
        /// <param name="fight"></param>
        /// <param name="caster"></param>
        /// <param name="castInfos"></param>
        /// <param name="cell"></param>
        /// <param name="duration"></param>
        /// <param name="actionId"></param>
        /// <param name="canGoThrough"></param>
        /// <param name="canStack"></param>
        /// <param name="hide"></param>
        protected AbstractActivableObject(FightObstacleTypeEnum type, ActiveType activeType, AbstractFight fight, AbstractFighter caster, CastInfos castInfos, int cell, int duration, int actionId, bool canGoThrough, bool canStack, bool hide = false)
        {
            m_fight = fight;
            m_caster = caster;
            m_spellId = castInfos.SpellId;
            m_actionSpell = SpellManager.Instance.GetTemplate(castInfos.Value1);
            m_actionEffect = m_actionSpell.GetLevel(castInfos.Value2);

            Cell = fight.GetCell(cell);
            ObstacleType = type;
            ActivationType = activeType;
            CanGoThrough = canGoThrough;
            CanStack = canStack;
            Color = castInfos.Value3;
            Targets = new List<AbstractFighter>();
            Length = Pathfinding.GetDirection(castInfos.RangeType[1]);
            AffectedCells = new List<FightCell>();
            Duration = duration;
            ActionId = actionId;
            Hide = hide;
                        
            foreach(var effect in m_actionEffect.Effects)
            {
                if(CastInfos.IsDamageEffect(effect.TypeEnum))
                {
                    Priority--;
                }
            }

            // On ajout l'objet a toutes les cells qu'il affecte
            foreach (var cellId in CellZone.GetCircleCells(fight.Map, cell, Length))
            {
                var fightCell = m_fight.GetCell(cellId);
                if (fightCell != null)
                {
                    fightCell.AddObject(this);
                    AffectedCells.Add(fightCell);
                }
            }

            if (Hide)
                Appear(caster.Team);
            else
                AppearForAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void LoadTargets(AbstractFighter target)
        {
            if(!Targets.Contains(target))
                Targets.Add(target);

            switch (ActivationType)
            {
                case ActiveType.ACTIVE_ENDMOVE:
                    foreach (var cell in AffectedCells)
                    {
                        Targets.AddRange(cell.FightObjects.OfType<AbstractFighter>().Where(fighter => !Targets.Contains(fighter)));
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activator"></param>
        public void Activate(AbstractFighter activator)
        {
            Activated = true;

            m_fight.CurrentProcessingFighter = activator;
            m_fight.Dispatch(WorldMessage.GAME_ACTION(ActionId, activator.Id, m_spellId + "," + Cell.Id + "," + m_actionSpell.Sprite + "," + m_actionEffect.Level + ",1," + m_caster.Id));

            foreach (var target in Targets)
            {
                if (!target.IsFighterDead)
                {
                    foreach (var effect in m_actionEffect.Effects)
                    {
                        m_fight.AddProcessingTarget(new CastInfos(
                                            effect.TypeEnum,
                                            m_spellId,
                                            Cell.Id,
                                            effect.Value1,
                                            effect.Value2,
                                            effect.Value3,
                                            effect.Chance,
                                            effect.Duration,
                                            m_caster,
                                            target,
                                            "",
                                            target.Cell.Id,
                                            isTrap: ObstacleType == FightObstacleTypeEnum.TYPE_TRAP)
                                         );
                    }
                }
            }

            Targets.Clear();

            if (ObstacleType == FightObstacleTypeEnum.TYPE_TRAP)
                Remove();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DecrementDuration()
        {
            Duration--;

            if (Duration <= 0)
                Remove();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            DisappearForAll();

            foreach (var cell in AffectedCells)
            {
                cell.RemoveObject(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(IFightObstacle obj)
        {
            return Priority.CompareTo(obj.Priority);
        }
    }
}
