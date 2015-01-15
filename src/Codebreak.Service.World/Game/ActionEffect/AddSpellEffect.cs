﻿using Codebreak.Service.World.Game.Entity;
using Codebreak.Service.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.ActionEffect
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddSpellEffect : ActionEffectBase<AddSpellEffect>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="item"></param>
        /// <param name="effect"></param>
        /// <param name="targetId"></param>
        /// <param name="targetCell"></param>
        /// <returns></returns>
        public override bool ProcessItem(CharacterEntity character, Database.Structure.InventoryItemDAO item, Stats.GenericStats.GenericEffect effect, long targetId, int targetCell)
        {
            return Process(character, new Dictionary<string, string>() { { "spellId", effect.Items.ToString() } });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override bool Process(CharacterEntity character, Dictionary<string, string> parameters)
        {
            var spellId = int.Parse(parameters["spellId"]);

            if (character.Spells.HasSpell(spellId))
            {
                character.Dispatch(WorldMessage.IM_ERROR_MESSAGE(InformationEnum.ERROR_UNABLE_LEARN_SPELL, spellId));
                return false;
            }

            character.Spells.AddSpell(spellId);
            character.Dispatch(WorldMessage.SPELLS_LIST(character.Spells));

            return true;
        }
    }
}