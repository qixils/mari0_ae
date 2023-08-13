using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ConnectorLib;
using ConnectorLib.JSON;
using ConnectorLib.SimpleTCP;
using CrowdControl.Common;
using Newtonsoft.Json;
using ConnectorType = CrowdControl.Common.ConnectorType;
using EffectResponse = ConnectorLib.JSON.EffectResponse;
using EffectStatus = CrowdControl.Common.EffectStatus;
using Log = CrowdControl.Common.Log;
using LogLevel = CrowdControl.Common.LogLevel;
using static System.Linq.Enumerable;

namespace CrowdControl.Games.Packs;

public class Mari0 : SimpleTCPPack<SimpleTCPServerConnector>
{
    public override string Host => "127.0.0.1";
    public override ushort Port => 58435;

    public Mari0(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) {}

    public override Game Game => new("Mari0", "Mari0", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects => new Effect[]
    {
        new("Invert D-Pad", "invert_wasd") { Price = 200, Duration = 15, Category = "Movement", Description = "Swaps left, right, up and down" },
        new("Swap Buttons", "invert_buttons") { Price = 200, Duration = 15, Category = "Movement", Description = "Swaps the run, jump, and portal buttons" },
        new("Randomize Outfit", "randomize_outfit") { Price = 20, Category = "Player", Description = "Randomizes Mario's color palette and accessories" },
        new("Add a Hat", "hat_give") { Price = 10, Category = "Player", Description = "Places a random silly hat on Mario's head" },
        new("Take a Hat", "hat_take") { Price = 10, Category = "Player", Description = "Takes a hat right off the top of Mario's head" },
    };
}