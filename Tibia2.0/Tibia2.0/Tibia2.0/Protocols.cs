using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tibia2._0 {
    public enum Protocol
    {
        Disconnected = 0,
        Connected = 1,
        CreateAccount = 2,
        CreateAccountResult = 3,
        LoginAccount = 4,
        LoginAccountResult = 5,
        CreateCharacter = 6,
        CreateCharacterResult = 7,
        LoginCharacter = 8,
        LoginCharacterResult = 9,
        Update = 10,
        Updated = 11,
        Move = 12,
        Moved = 13
    }
}
