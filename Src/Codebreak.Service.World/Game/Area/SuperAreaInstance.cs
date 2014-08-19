﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codebreak.Service.World.Database.Structures;

namespace Codebreak.Service.World.Game.Area
{
    public sealed  class SuperAreaInstance : MessageDispatcher
    {
        private SuperAreaDAO _superAreaRecord;

        public SuperAreaInstance(SuperAreaDAO record)
        {
            _superAreaRecord = record;
        }
    }
}