﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.WorldService.World.Fight.Effect.Type
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PunishmentEffect : EffectBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public override FightActionResultEnum ApplyEffect(CastInfos CastInfos)
        {
            if (CastInfos.Target == null)
                return FightActionResultEnum.RESULT_NOTHING;

            CastInfos.Target.BuffManager.AddBuff(new PunishmentBuff(CastInfos, CastInfos.Target));

            return FightActionResultEnum.RESULT_NOTHING;
        }
    }
}