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

namespace CrowdControl.Games.Packs.Mari0;

public class Mari0 : SimpleTCPPack<SimpleTCPServerConnector>
{
	public override string Host => "127.0.0.1";
	public override ushort Port => 28379;

	public Mari0(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

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
		new("Supersize Enemies", "supersize_enemies") { Price = 50, Category = new("Spawns", "Enemies"), Description = "Supersizes all supported on-screen enemies" },
		new("Kill Enemies", "kill_enemies") { Price = 80, Category = new("Spawns", "Enemies"), Description = "Kills all supported on-screen enemies" },
		new("Restart Level", "restart_level") { Price = 150, Category = new("Player", "Level"), Description = "Restarts the current level" },
		new("Lights Out", "enable_lightsout") { Price = 75, Duration = 10, Category = "Level", Description = "Enables the lights out effect" },
		new("Lights On", "disable_lightsout") { Price = 50, Duration = 10, Category = "Level", Description = "Disables the lights out effect" },
		new("Give Extra Life", "add_life") { Price = 50, Quantity = 100, Category = "Player", Description = "Gives Mario an extra life" },
		new("Take Extra Life", "take_life") { Price = 100, Quantity = 100, Category = "Player", Description = "Steals an extra life from Mario" },
		new("Kill Player", "kill_player") { Price = 250, Category = "Player", Description = "Murder Mario." },
		new("Can't Move", "stun_player") { Price = 150, Duration = 5, Category = "Movement", Description = "Stuns Mario, freezing him in place for a few seconds" },
		new("Auto Walk", "auto_walk") { Price = 175, Duration = 10, Category = "Movement", Description = "Force Mario to auto-walk through the level" },
		new("Auto Run", "auto_run") { Price = 200, Duration = 10, Category = "Movement", Description = "Force Mario to auto-run through the level" },
		new("Flip Gravity", "flip_gravity") { Price = 200, Duration = 10, Category = "Level", Description = "Inverts the levels gravity" },
		new("Speedup Game", "speedup_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds up the game to x1.5" },
		new("Slowdown Game", "slowdown_game") { Price = 150, Duration = 10, Category = "Level", Description = "Speeds down the game to x0.67" },
		new("Shader Shuffle", "shader_1") { Note = "Slot 1", Price = 50, Category = "Visual", Description = "Selects a random shader in the first slot" },
		new("Shader Shuffle", "shader_2") { Note = "Slot 2", Price = 50, Category = "Visual", Description = "Selects a random shader in the second slot" },
		new("Shader Reset", "shader_clear") { Price = 40, Category = "Visual", Description = "Clears all active shaders" },
		new("Goomba Attack", "goomba_attack") { Price = 125, Duration = 20, Category = "Level", Description = "Enables the Goomba Attack cheat, spawning random goombas as the player walks through the level" },
		new("Gamemode: Minecraft", "minecraft") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with the pickaxe from Minecraft, which can be used to break and place tiles" },
		new("Gamemode: Gelcannon", "gelcannon") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with the Gel Cannon, which fires out various gels for bouncing and increasing speed" },
		// new("Gamemode: Cappy", "cappy") { Price = 100, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with Cappy from Super Mario Odyssey, which can be used to gain height and capture enemies" },
		new("Gamemode: Classic", "classic") { Price = 150, Duration = 30, Category = "Player", Description = "Replaces the Portal Gun with nothing! All the player can do is run and jump!" },
		new("Become 3D", "3d") { Price = 75, Duration = 30, Category = "Visual", Description = "Turns the game 3-dimensional" },
		new("Time Add", "time_add") { Price = 2, Quantity = 60, Category = "Level", Description = "Adds real-time to the in-game timer" },
		new("Time Remove", "time_sub") { Price = 5, Quantity = 60, Category = "Level", Description = "Removes real-time from the in-game timer" },
		new("Bonk", "bonk") { Price = 25, Category = "Player", Description = "Halts Mario's vertical momentum as if he hit an invisible block" },
		new("Starman", "star") { Price = 50, Category = "Player", Description = "Grants Mario a Super Star! Yahoo!" },
		new("Disable Screen Scroll", "disable_scroll") { Price = 50, Duration = 10, Category = "Level", Description = "Prevents the screen from scrolling for a short time" },
		new("Deadly Coins", "deadly_coin") { Price = 100, Duration = 15, Category = "Level", Description = "Makes collected coins kill the player for a short time" },
		new("Coin Give", "coin_add") { Price = 2, Quantity = 99, Category = "Level", Description = "Gives some coins which might contribute to a 1-Up!" },
		new("Coin Take", "coin_sub") { Price = 2, Quantity = 99, Category = "Level", Description = "Take some coins away from contributing to a 1-Up!" },
		new("Underwater", "underwater") { Price = 100, Duration = 15, Category = new("Player", "Level"), Description = "Sets the player to swim through the sky!" },
		new("Flying Fish", "flyingfish") { Price = 50, Duration = 15, Category = new("Level", "Spawns", "Enemies"), Description = "Spawns fish that fly from the bottom of the screen for a short time" },
		new("Meteor Shower", "meteorshower") { Price = 50, Duration = 15, Category = new("Level", "Spawns", "Enemies"), Description = "Spawns meteors that destroy tiles from the top of the screen for a short time" },
		new("Bullet Bill Storm", "bulletbills") { Price = 50, Duration = 15, Category = new("Level", "Spawns", "Enemies"), Description = "Spawns bullet bills from the edge of the screen for a short time" },
		new("Windy", "wind") { Price = 50, Duration = 15, Category = "Level", Description = "Spawns a gust of wind that pushes Mario for a short time" },
		new("Bowser Fire", "bowserfire") { Price = 75, Duration = 15, Category = new("Level", "Spawns", "Enemies"), Description = "Spawns Bowser's fire from the edge of the screen for a short time" },
		// new("Huge Mario", "bigmario") { Price = 100, Duration = 15, Category = "Player", Description = "Turns Mario huge for a short time" }, // graphics corruption
		new("Frames Per Minute", "lowfps") { Price = 200, Duration = 15, Category = "Level", Description = "Lowers the FPS (and physics accuracy!) drastically for a short time" },
		new("Cover in Bouncy Gel", "gel_1") { Price = 100, Category = "Level", Description = "Cover 75% of surfaces with bouncy repulsion gel" },
		new("Cover in Speedy Gel", "gel_2") { Price = 100, Category = "Level", Description = "Cover 75% of surfaces with speedy propulsion gel" },
		new("Cover in Gravity Gel", "gel_4") { Price = 100, Category = "Level", Description = "Cover 75% of surfaces with gravity-sticking adhesion gel" },

		// Spawning Effects
		// goomba koopa lavabubble amp fuzzy angrysun
		new("Spawn Amp", "spawn_amp") { Price = 20, Category = new("Spawns", "Enemies"), Description = "Spawns an amp that shocks the player when touched" },
		new("Spawn Angry Sun", "spawn_angrysun") { Price = 50, Category = new("Spawns", "Enemies"), Description = "Spawns an angry sun that flies across the screen to attack" },
		new("Spawn Barrel", "spawn_barrel") { Price = 10, Category = new("Spawns", "Enemies"), Description = "Spawns a barrel that randomly rolls around to attack" },
		new("Spawn Big Bill", "spawn_bigbill") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns a big bullet bill that fires at the player" },
		new("Spawn Big Mole", "spawn_bigmole") { Price = 35, Category = new("Spawns", "Enemies"), Description = "Spawns a big mole that runs randomly around to attack" },
		new("Spawn Bob-Omb", "spawn_bomb") { Price = 35, Category = new("Spawns", "Enemies"), Description = "Spawns a bob-omb that explodes shortly after it's jumped on" },
		new("Spawn Boo", "spawn_boo") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns a ghost that chases Mario while he's looking away" },
		// new("Spawn Boom Boom", "spawn_boomboom") { Price = 120, Category = new("Spawns", "Enemies"), Description = "Spawns the Boom Boom boss which runsa round chaotically" }, // ends level on death
		// new("Spawn Bowser", "spawn_bowser") { Price = 100, Category = new("Spawns", "Enemies"), Description = "Spawns the Bowser boss which shoots flames" }, // just shuffles around awkwardly
		// new("Spawn Bowser Fire", "spawn_fire") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a flame from Bowser's mouth" }, // inferior to timed effect
		// new("Spawn Bullet Bill", "spawn_bulletbill") { Price = 45, Category = new("Spawns", "Enemies"), Description = "Spawns a bullet bill that fires at the player" }, // inferior to timed effect
		new("Spawn Cannonball", "spawn_cannonball") { Price = 20, Category = new("Spawns", "Enemies"), Description = "Throws a cannonball at the player" },
		new("Spawn Chain Chomp", "spawn_chainchomp") { Price = 50, Category = new("Spawns", "Enemies"), Description = "Spawns a Chained enemy that'll tug on its leash to try to Chomp the player" },
		// new("Spawn Cheep Cheep", "spawn_cheepcheep") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns a fish that moves back and forth" }, // just doesn't work
		new("Spawn Companion Cube", "spawn_box") { Price = 2, Category = new("Spawns", "Objects"), Description = "Drops a random cube or core on the player. It doesn't do any damage but it sure is in the way." },
		new("Spawn Donut Block", "spawn_donut") { Price = 2, Category = new("Spawns", "Objects"), Description = "Spawns a block that falls when stood on" },
		new("Spawn Dry Bones", "spawn_drybones") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns a monster that wanders like a Koopa but gets back up after attacked" },
		new("Spawn Fireballs (Friendly)", "spawn_fireball") { Price = 5, Category = new("Spawns", "Allies"), Description = "Unload several fireballs against Mario's enemies" },
		// new("Spawn Fireballs (Hostile)", "spawn_brofireball") { Price = 10, Category = new("Spawns", "Enemies"), Description = "Unload several fireballs against Mario himself" }, // not actually very hostile
		new("Spawn Firebar", "spawn_castlefire") { Price = 60, Category = new("Spawns", "Enemies"), Description = "Spawns a rotating bar of flames" },
		new("Spawn Fishbone", "spawn_fishbone") { Price = 35, Category = new("Spawns", "Enemies"), Description = "Spawns a fishbone that swims back and forth until it spots Mario and lunges at him" },
		new("Spawn Flip Block", "spawn_flipblock") { Price = 2, Category = new("Spawns", "Enemies"), Description = "Spawns a block that temporarily deactivates when hit" },
		// new("Spawn Flying Fish", "spawn_flyingfish") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a fish that leaps once into the sky then back down into the water" }, // inferior to timed effect
		new("Spawn Fuzzy", "spawn_fuzzy") { Price = 20, Category = new("Spawns", "Enemies"), Description = "Spawns a fuzzy that thorns the player when touched" },
		// new("Spawn GLaDOS", "spawn_glados") { Price = 150, Category = new("Spawns", "Enemies"), Description = "Spawns the GLaDOS boss which can be defeated with a Rocket Turret" }, // very situational
		new("Spawn Goomba", "spawn_goomba") { Price = 25, Category = new("Spawns", "Enemies"), Description = "Spawns one of several random goombas with different behaviors" },
		new("Spawn Grinder", "spawn_grinder") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a large saw that injures Mario on touch" },
		new("Spawn Hammer Bro", "spawn_hammerbro") { Price = 75, Category = new("Spawns", "Enemies"), Description = "Spawns a Bro that throws Hammers at Mario" },
		new("Spawn Homing Fire", "spawn_plantfire") { Price = 10, Category = new("Spawns", "Enemies"), Description = "Spawns a fireball aimed directly at Mario" },
		new("Spawn Icicle", "spawn_icicle") { Price = 15, Category = new("Spawns", "Enemies"), Description = "Spawns an icicle that falls when Mario gets close" },
		new("Spawn King Bill", "spawn_kingbill") { Price = 80, Category = new("Spawns", "Enemies"), Description = "Spawns a King Bill that shoots at Mario destroying blocks in its path" },
		new("Spawn Koopa", "spawn_koopa") { Price = 25, Category = new("Spawns", "Enemies"), Description = "Spawns one of several random koopas with different behaviors" },
		new("Spawn Lakitu", "spawn_lakito") { Price = 100, Category = new("Spawns", "Enemies"), Description = "Spawns a Lakitu who flies around throwing Spiny enemies at Mario" },
		new("Spawn Lava Bubble", "spawn_upfire") { Price = 25, Category = new("Spawns", "Enemies"), Description = "Spawns a Bubble of Lava that randomly leaps up from the bottom of the screen" },
		new("Spawn Magikoopa", "spawn_magikoopa") { Price = 75, Category = new("Spawns", "Enemies"), Description = "Spawns a Magikoopa wizard who randomly teleports around casting spells at Mario and his surroundings" },
		// new("Spawn Meteor", "spawn_meteor") { Price = 60, Category = new("Spawns", "Enemies"), Description = "Spawns a Meteor that rains down from the sky and explodes blocks" }, // inferior to timed effect
		new("Spawn Minecraft Block", "spawn_miniblock") { Price = 1, Category = new("Spawns", "Objects"), Description = "Spawns a mini Minecraft block. Can be collected and placed with the Gamemode: Minecraft effect active." },
		new("Spawn Mole", "spawn_mole") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a Mole that runs around randomly to attack" },
		new("Spawn Muncher", "spawn_muncher") { Price = 20, Category = new("Spawns", "Enemies"), Description = "Spawns a Muncher that chomps Mario when touched" },
		// new("Spawn Mushroom", "spawn_mushroom") { Price = 15, Category = new("Spawns", "Enemies"), Description = "Spawns a Mushroom that powers up Mario when touched" }, // duplicate of randomize powerup
		new("Spawn Ninji", "spawn_ninji") { Price = 25, Category = new("Spawns", "Enemies"), Description = "Spawns in a little Ninji who jumps up and down as Mario approaches" },
		new("Spawn POW Block", "spawn_powblock") { Price = 5, Category = new("Spawns", "Objects"), Description = "Spawns a POW block which destroys nearby enemies when hit by a shell (difficult)" },
		new("Spawn Para Beetle", "spawn_parabeetle") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns in a paragliding beetle who flies in a random direction at a random speed" },
		new("Spawn Piranha Plant", "spawn_plant") { Price = 60, Category = new("Spawns", "Enemies"), Description = "Spawns in a random Piranha Plant to attack Mario from the \"ground\"" },
		new("Spawn Piranha Head", "spawn_plantcreeper") { Price = 20, Category = new("Spawns", "Enemies"), Description = "Spawns in a large head of a piranha plant" },
		new("Spawn Poison Mushroom", "spawn_poisonmush") { Price = 25, Category = new("Spawns", "Enemies"), Description = "Spawns in a poisonous mushroom that hurts Mario when touched" },
		new("Spawn Pokey", "spawn_pokey") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a Pokey, a large tower of spiky guys" },
		new("Spawn Rocket Turret", "spawn_rocketturret") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns a turret that slowly locks on to Mario before firing a rocket" },
		new("Spawn Rocky Wrench", "spawn_rockywrench") { Price = 35, Category = new("Spawns", "Enemies"), Description = "Spawns a mole who lives in the \"ground\" and peeks out to throw wrenches" },
		new("Spawn Sidestepper", "spawn_sidestepper") { Price = 35, Category = new("Spawns", "Enemies"), Description = "Spawns in a crab who crawls from side to side" },
		new("Spawn Skewer", "spawn_skewer") { Price = 60, Category = new("Spawns", "Enemies"), Description = "Spawns in a spiky skewer that slightly retracts before plunging forwards" },
		new("Spawn Small Spring", "spawn_smallspring") { Price = 5, Category = new("Spawns", "Objects"), Description = "Spawns a small spring that allows Mario to jump higher" },
		new("Spawn Spike", "spawn_spike") { Price = 40, Category = new("Spawns", "Enemies"), Description = "Spawns Spike, a guy who walks around and occasionally up-chucks a spikey ball at Mario" },
		new("Spawn Spring", "spawn_spring") { Price = 7, Category = new("Spawns", "Objects"), Description = "Spawns a large spring that allows Mario to jump even higher" },
		new("Spawn Squid", "spawn_squid") { Price = 75, Category = new("Spawns", "Enemies"), Description = "Spawns a squid who hovers around Mario" },
		// new("Spawn Star", "spawn_star") { Price = 50, Category = new("Spawns", "Enemies"), Description = "Spawns a star that gives Mario a short burst of invincibility from enemies" }, // duplicate of starman
		new("Spawn Thwomp", "spawn_thwomp") { Price = 70, Category = new("Spawns", "Enemies"), Description = "Spawns a Thwomp who slams down when Mario is nearby" },
		new("Spawn Torpedo Ted", "spawn_torpedoted") { Price = 30, Category = new("Spawns", "Enemies"), Description = "Spawns a long but short torpedo to fire at Mario" },
		new("Spawn Turret", "spawn_turret") { Price = 150, Category = new("Spawns", "Enemies"), Description = "Spawns a turret who quickly shoots at Mario when he's in sight" },
	};
}
