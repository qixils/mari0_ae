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
        new("Supersize Enemies", "supersize_enemies") { Price = 50, Category = "Enemies", Description = "Supersizes all supported on-screen enemies" },
        new("Kill Enemies", "kill_enemies") { Price = 75, Category = "Enemies", Description = "Kills all supported on-screen enemies" },
        new("Restart Level", "restart_level") { Price = 150, Category = new("Player", "Level"), Description = "Restarts the current level" },
        new("Lights Out", "enable_lightsout") { Price = 50, Duration = 10, Category = "Level", Description = "Enables the lights out effect" },
        new("Lights On", "disable_lightsout") { Price = 75, Duration = 10, Category = "Level", Description = "Disables the lights out effect" },
        new("Give Extra Life", "add_life") { Price = 50, Quantity = 100, Category = "Player", Description = "Gives Mario an extra life" },
        new("Take Extra Life", "take_life") { Price = 100, Quantity = 100, Category = "Player", Description = "Steals an extra life from Mario" },
        new("Kill Player", "kill_player") { Price = 250, Category = "Player", Description = "Murder Mario." },
        new("Can't Move", "stun_player") { Price = 150, Duration = 5, Category = "Movement", Description = "Stuns Mario, freezing him in place for a few seconds" },
        new("Auto Walk", "auto_walk") { Price = 175, Duration = 10, Category = "Movement", Description = "Force Mario to auto-walk through the level" },
        new("Auto Run", "auto_run") { Price = 200, Duration = 10, Category = "Movement", Description = "Force Mario to auto-run through the level" },
        new("Flip Gravity", "flip_gravity") { Price = 175, Duration = 10, Category = "Level", Description = "Inverts the levels gravity" },
        new("Speedup Game", "speedup_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds up the game to x1.5" },
        new("Slowdown Game", "slowdown_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds down the game to x0.67" },
		new("Shader Shuffle", "shader_1") { Note = "Slot 1", Price = 50, Category = "Visual", Description = "Selects a random shader in the first slot" },
		new("Shader Shuffle", "shader_2") { Note = "Slot 2", Price = 50, Category = "Visual", Description = "Selects a random shader in the second slot" },
		new("Shader Reset", "shader_clear") { Price = 40, Category = "Visual", Description = "Clears all active shaders" },

        new("Spawn Goomba", "spawn_goomba") { Price = 25, Category = "Enemies", Description = "Spawn an enemy on the screen!" },
        new("Spawn Koopa", "spawn_koopa") { Price = 25, Category = "Enemies", Description = "Spawn an enemy on the screen!" },
    };
}
