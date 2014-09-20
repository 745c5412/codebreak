﻿using System;

namespace Codebreak.RPC.Protocol
{
    [Flags]
    public enum MessageId : int
    {
        WORLD_TO_AUTH_CREDENTIAL,
        AUTH_TO_WORLD_CREDENTIALRESULT,
        WORLD_TO_AUTH_GAMEIDUPDATE,
        WORLD_TO_AUTH_GAMESTATEUPDATE,
        AUTH_TO_WORLD_GAMETICKET,
        WORLD_TO_AUTH_GAMEACCOUNTDISCONNECTED,
    }
}
