﻿using System;
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

namespace CrowdControl.Games.Packs.Mari0;

public class Mari0 : SimpleTCPPack<SimpleTCPServerConnector>
{
    public override string Host => "127.0.0.1";
    public override ushort Port => 28379;

    public Mari0(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) {}

    public override Game Game => new("Mari0", "Mari0", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects => new Effect[]
    {
        new("Invert D-Pad", "invert_wasd") { Price = 150, Duration = 15, Category = "Movement", Description = "Swaps left, right, up and down" },
        new("Swap Buttons", "invert_buttons") { Price = 150, Duration = 15, Category = "Movement", Description = "Swaps the run, jump, and portal buttons" },
        new("Randomize Outfit", "randomize_outfit") { Price = 15, Category = "Player", Description = "Randomizes Mario's color palette and accessories" },
        new("Add a Hat", "hat_give") { Price = 10, Category = "Player", Description = "Places a random silly hat on Mario's head" },
        new("Take a Hat", "hat_take") { Price = 10, Category = "Player", Description = "Takes a hat right off the top of Mario's head" },
        new("Randomize Powerup", "randomize_powerup") { Price = 25, Category = "Player", Description = "Randomize Mario's current powerup" },
        new("Remove Powerup", "remove_powerup") { Price = 25, Category = "Player", Description = "Takes Mario's current powerup" },
        new("Supersize Enemies", "supersize_enemies") { Price = 75, Category = "Enemies", Description = "Supersizes all supported on-screen enemies" },
        new("Kill Enemies", "kill_enemies") { Price = 75, Category = "Enemies", Description = "Kills all supported on-screen enemies" },
        new("Restart Level", "restart_level") { Price = 150, Category = new("Player", "Level"), Description = "Restarts the current level" },
        new("Give Extra Life", "add_life") { Quantity = 100, Price = 50, Category = "Player", Description = "Gives Mario an extra life" },
        new("Take Extra Life", "take_life") { Quantity = 100, Price = 100, Category = "Player", Description = "Steals an extra life from Mario" },
    };
}