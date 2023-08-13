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
        new("Invert Movement", "invert_wasd") { Price = 200, Duration = 15, Category = "Movement", Description = "Temporarily inverts WASD movement" },
    };
}