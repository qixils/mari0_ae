cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
old_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

-- misc state variables
wasd_inverted = false
buttons_inverted = false
toggled_lightsout = false
gravity_flipped = false

ccentitycreators = {
    goomba = function(x, y)
        local n = math.random(61)
        local type = nil
		if n >= 58 then
			table.insert(objects["splunkin"], splunkin:new(x, y))
			return true
        elseif n >= 55 then
            type = "tinygoomba"
        elseif n >= 52 then
            type = "paragoomba"
        elseif n >= 51 then
            type = "drygoomba"
        elseif n >= 47 then
            type = "biggoomba"
        elseif n >= 41 then
            type = "goombrat"
        end
		table.insert(objects["goomba"], goomba:new(x - .5, y, type))
    end,
    koopa = function(x, y)
        local n = math.random(60)
        local type = nil
        -- red redflying flying2 flying beetle beetleshell downbeetle downspikey spikeyshell blue shell bigkoopa bigbeetle
        if n >= 56 then
            type = "bigkoopa"
        elseif n >= 54 then
            type = "redflying"
        elseif n >= 52 then
            type = "flying2"
        elseif n >= 50 then
            type = "flying"
        elseif n >= 46 then
            type = "blue" -- fast
        elseif n >= 30 then
            type = "red"
        end
        table.insert(objects["koopa"], koopa:new(x - .5, y, type))
    end,
	checkpoint = function(x, y)
		-- TODO: needs to be persistent
		if not map[x] then map[x] = {} end
		map[x][y] = {} -- TOOD: IDK?
		table.insert(checkpoints, x)
		checkpointpoints[x] = y
		objects["checkpointflag"][x] = checkpointflag:new(x, y, {}, #checkpoints)
	end,
	box = function(x, y)
		if math.random(2) == 1 then
			local types = {"box", "box2", "edgeless"}
			local t = types[math.random(#types)]
			local spawned = box:new(x, y, t)
			if math.random(5) == 1 then
				local gels = {1, 2, 4}
				local gel = gels[math.random(#gels)]
				spawned:globalcollide("gel", { id = gel })
			end
			table.insert(objects["box"], spawned)
		else
			local types = {"curiosity", "cakemix", "anger", "morality"}
			local t = types[math.random(#types)]
			table.insert(objects["core"], core:new(x, y, t))
		end
	end,
	upfire = function(x, y)
		table.insert(objects["upfire"], upfire:new(x, y))
	end,
	amp = function(x, y)
		table.insert(objects["amp"], amp:new(x, y, "0|amp"))
	end,
	fuzzy = function(x, y)
		table.insert(objects["amp"], amp:new(x, y, "0|fuzzy"))
	end,
	angrysun = function()
		if angrysunend then return false end
		table.insert(objects["angrysun"], angrysun:new(0, 2))
	end,
	bowser = function(x, y)
		table.insert(objects["bowser"], bowser:new(x, y))
	end,
	-- NEW ENEMIES AND ENTITIES
	magikoopa = function(x, y)
		table.insert(objects["magikoopa"], magikoopa:new(x, y))
	end,
	thwomp = function(x, y)
		local types = {"down", "left", "right", "thwimp"}
		local t = types[math.random(#types)]
		table.insert(objects["thwomp"], thwomp:new(x, y, t))
	end,
	hammerbro = function(x, y)
		table.insert(objects["hammerbro"], hammerbro:new(x, y))
	end,
	boo = function(x, y)
		table.insert(objects["boo"], boo:new(x, y))
	end,
	chainchomp = function(x, y)
		table.insert(objects["chainchomp"], chainchomp:new(x, y))
	end,
	cheepcheep = function(x, y)
		table.insert(objects["cheepcheep"], cheepcheep:new(x, y, math.random(3)))
	end,
	drybones = function(x, y)
		table.insert(objects["drybones"], drybones:new(x, y))
	end,
	fire = function(x, y)
		table.insert(objects["fire"], fire:new(x, y))
	end,
	bomb = function(x, y)
		table.insert(objects["bomb"], bomb:new(x, y))
	end,
	bulletbill = function(x, y)
		table.insert(objects["bulletbill"], bulletbill:new(x, y, "left"))
	end,
	cannonball = function(x, y)
		table.insert(objects["cannonball"], cannonball:new(x, y, "left"))
	end,
	castlefire = function(x, y)
		table.insert(objects["castlefire"], castlefire:new(x, y))
	end,
	fishbone = function(x, y)
		table.insert(objects["fishbone"], fishbone:new(x, y, 1))
	end,
	flyingfish = function(x, y)
		table.insert(objects["flyingfish"], flyingfish:new())
	end,
	glados = function(x, y)
		table.insert(objects["glados"], glados:new(x, y))
	end,
	grinder = function(x, y)
		table.insert(objects["grinder"], grinder:new(x, y))
	end,
	icicle = function(x, y)
		local types = {"big", "small"}
		local t = types[math.random(#types)]
		table.insert(objects["icicle"], icicle:new(x, y, t))
	end,
	kingbill = function(x, y)
		table.insert(objects["kingbill"], kingbill:new(x, y, nil, "left"))
	end,
	lakito = function(x, y)
		table.insert(objects["lakito"], lakito:new(x, y))
	end,
	meteor = function(x, y)
		table.insert(objects["meteor"], meteor:new())
	end,
	mole = function(x, y)
		local types = {"big", "small"}
		local t = types[math.random(#types)]
		table.insert(objects["mole"], mole:new(x, y, t))
	end,
	muncher = function(x, y)
		table.insert(objects["muncher"], muncher:new(x, y))
	end,
	ninji = function(x, y)
		table.insert(objects["ninji"], ninji:new(x, y))
	end,
	parabeetle = function(x, y)
		local types = {"parabeetle", "parabeetleright", "parabeetlegreen", "parabeetlegreenright"}
		local t = types[math.random(#types)]
		table.insert(objects["parabeetle"], parabeetle:new(x, y, t))
	end,
	plant = function(x, y)
		local types = {"plant", "redplant", "dryplant", "fireplant"}
		local t = types[math.random(#types)]
		local dirs = {"up", "down", "left", "right"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["plant"], plant:new(x, y, t, dir))
	end,
	plantcreeper = function(x, y)
		table.insert(objects["plantcreeper"], plantcreeper:new(x, y, {}))
	end,
	plantfire = function(x, y)
		table.insert(objects["plantfire"], plantfire:new(x, y))
	end,
	poisonmush = function(x, y)
		table.insert(objects["poisonmush"], poisonmush:new(x, y))
	end,
	pokey = function(x, y)
		table.insert(objects["pokey"], pokey:new(x, y))
	end,
	rocketturret = function(x, y)
		table.insert(objects["rocketturret"], rocketturret:new(x, y))
	end,
	rockywrench = function(x, y)
		table.insert(objects["rockywrench"], rockywrench:new(x, y))
	end,
	sidestepper = function(x, y)
		table.insert(objects["sidestepper"], sidestepper:new(x, y))
	end,
	skewer = function(x, y)
		local dirs = {"up", "down", "left", "right"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["skewer"], skewer:new(x, y, dir, {}))
	end,
	spike = function(x, y)
		table.insert(objects["spike"], spike:new(x, y))
	end,
	squid = function(x, y)
		local types = {"pink", "normal"}
		local t = types[math.random(#types)]
		table.insert(objects["squid"], squid:new(x, y, t))
	end,
	torpedoted = function(x, y)
		table.insert(objects["torpedoted"], torpedoted:new(x, y, "left"))
	end,
	turret = function(x, y)
		table.insert(objects["turret"], turret:new(x, y))
	end,
	barrel = function(x, y)
		table.insert(objects["barrel"], barrel:new(x, y))
	end,
	bigbill = function(x, y)
		table.insert(objects["bigbill"], bigbill:new(x, y, "left"))
	end,
	bigmole = function(x, y)
		table.insert(objects["bigmole"], bigmole:new(x, y))
	end,
	boomboom = function(x, y)
		table.insert(objects["boomboom"], boomboom:new(x, y))
	end,
	-- PROJECTILES AND ITEMS
	fireball = function(x, y)
		local types = {"fireball", "iceball", "superball"}
		local dirs = {"left", "right"}
		for i = 1, 7 do
			local t = types[math.random(#types)]
			local dir = dirs[math.random(#dirs)]
			local offsetx = x + math.random(-2, 2)
			local offsety = y + math.random(-2, 2)
			table.insert(objects["fireball"], fireball:new(offsetx, offsety, dir, nil, t))
		end
	end,
	brofireball = function(x, y)
		local types = {"fire", "ice"}
		for i = 1, 3 do
			local t = types[math.random(#types)]
			local offsetx = x + math.random(-2, 2)
			local offsety = y + math.random(-2, 2)
			table.insert(objects["fireball"], brofireball:new(offsetx, offsety, "left", t))
		end
	end,
	ice = function(x, y)
		table.insert(objects["ice"], ice:new(x, y, 1, 1, 1))
	end,
	laser = function(x, y)
		local dirs = {"right", "left", "up", "down"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["laser"], laser:new(x, y, dir, {1, 1}))
	end,
	lightbridge = function(x, y)
		local dirs = {"right", "left", "up", "down"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["lightbridge"], lightbridge:new(x, y, dir, {1, 1}))
	end,
	portal = function(x, y)
		table.insert(objects["portal"], portal:new(x, y))
	end,
	vine = function(x, y)
		table.insert(objects["vine"], vine:new(x, y, "start"))
	end,
	yoshi = function(x, y)
		local colors = {1, 2, 3, 4}
		local color = colors[math.random(#colors)]
		table.insert(objects["yoshi"], yoshi:new(x, y, color))
	end,
	mushroom = function(x, y)
		table.insert(objects["mushroom"], mushroom:new(x, y))
	end,
	star = function(x, y)
		table.insert(objects["star"], star:new(x, y))
	end,
	coin = function(x, y)
		table.insert(objects["coin"], coin:new(x, y))
	end,
	cappy = function(x, y)
		table.insert(objects["cappy"], cappy:new(x, y))
	end,
	mariohammer = function(x, y)
		local dir = math.random(2) == 1 and "left" or "right"
		table.insert(objects["mariohammer"], mariohammer:new(x, y, dir, nil))
	end,
	mariotail = function(x, y)
		local dir = math.random(2) == 1 and "left" or "right"
		table.insert(objects["mariotail"], mariotail:new(x, y, dir, nil))
	end,
	boomerang = function(x, y)
		local dir = math.random(2) == 1 and "left" or "right"
		table.insert(objects["boomerang"], boomerang:new(x, y, dir, nil))
	end,
	gel = function(x, y)
		local types = {1, 2, 4}
		for i = 1, 5 do
			local t = types[math.random(#types)]
			local spawned = gel:new(x, y, t)
			spawned.speedx = math.random(-10, 10)
			spawned.speedy = math.random(-10, 10)
			table.insert(objects["gel"], spawned)
		end
	end,
	geldispenser = function(x, y)
		local types = {1, 2, 4}
		local t = types[math.random(#types)]
		local dirs = {"down", "right", "left"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["geldispenser"], geldispenser:new(x, y, t, dir, {1, 1}))
	end,
	faithplate = function(x, y)
		local dirs = {"up", "right", "left"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["faithplate"], faithplate:new(x, y, dir, 1, {1, 1}))
	end,
	funnel = function(x, y)
		local dirs = {"right", "down", "left", "up"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["funnel"], funnel:new(x, y, dir, {1, 1}))
	end,
	cubedispenser = function(x, y)
		table.insert(objects["cubedispenser"], cubedispenser:new(x, y, {1, 1}))
	end,
	snakeblock = function(x, y)
		table.insert(objects["snakeblock"], snakeblock:new(x, y, {1, 1}))
	end,
	platform = function(x, y)
		table.insert(objects["platform"], platform:new(x, y, {1, 1}))
	end,
	seesaw = function(x, y)
		table.insert(objects["seesaw"], seesaw:new(x, y, {1, 1}))
	end,
	spring = function(x, y)
		table.insert(objects["spring"], spring:new(x, y))
	end,
	smallspring = function(x, y)
		table.insert(objects["smallspring"], smallspring:new(x, y))
	end,
	longfire = function(x, y)
		table.insert(objects["longfire"], longfire:new(x, y, {1, 1}))
	end,
	track = function(x, y)
		table.insert(objects["track"], track:new(x, y, {1, 1}, false))
	end,
	enemytool = function(x, y)
		local enemies = {"goomba", "koopa", "bulletbill", "lakito", "angrysun"}
		local enemy = enemies[math.random(#enemies)]
		table.insert(objects["enemytool"], enemytool:new(x, y, {1, 1, enemy}))
	end,
	tiletool = function(x, y)
		table.insert(objects["tiletool"], tiletool:new(x, y, {1, 1, 1}))
	end,
	button = function(x, y)
		local types = {1, 2, 3}
		local t = types[math.random(#types)]
		table.insert(objects["button"], button:new(x, y, t, {1, 1}))
	end,
	pushbutton = function(x, y)
		local dirs = {"left", "right"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["pushbutton"], pushbutton:new(x, y, dir, {1, 1}))
	end,
	door = function(x, y)
		local dirs = {"ver", "hor"}
		local dir = dirs[math.random(#dirs)]
		table.insert(objects["door"], door:new(x, y, {1, 1}, dir))
	end,
	clearpipe = function(x, y)
		table.insert(objects["clearpipe"], clearpipe:new(x, y, {1, 1}))
	end,
	donut = function(x, y)
		table.insert(objects["donut"], donut:new(x, y, nil, false, "false"))
	end,
	flipblock = function(x, y)
		table.insert(objects["flipblock"], flipblock:new(x, y))
	end,
	miniblock = function(x, y)
		table.insert(miniblocks, miniblock:new(x-.5, y-.2, math.random(#tilequads)))
	end,
	powblock = function(x, y)
		table.insert(objects["powblock"], powblock:new(x, y))
	end,
	seesawplatform = function(x, y)
		table.insert(objects["seesawplatform"], seesawplatform:new(x, y, "left"))
	end,
	windleaf = function(x, y)
		table.insert(objects["windleaf"], windleaf:new(x, y))
	end,
}

--- Determines if the given string starts with the given substring.
---@param str string The string to check.
---@param start string The substring to check for.
---@return boolean
function string.startswith(str, start)
    return string.sub(str, 1, string.len(start)) == start
end

--- Sends a message to the Crowd Control server.
---@param msg string|table The message to send. If a table is provided, it will be encoded as JSON.
function cc_send(msg)
    if type(msg) == "table" then
        msg = JSON:encode(msg)
    end
    outgoing:push(msg .. "\0")
end

--- Marks the given request as started.
function cc_start(request)
    if request.started then return end
    request.started = love.timer.getTime()
end

--- Gets an active request by the given effect ID and optionally marks it as acknowledged.
---@param effect string The effect to get.
---@param ack boolean? Whether to mark the effect as acknowledged.
---@return table|nil
function cc_get(effect, ack)
    for i, request in ipairs(cc_requests) do
        if request.code == effect then
            if ack then
                cc_start(request)
            end
            return request
        end
    end
    return nil
end

--- Checks if an effect is active without marking it as acknowledged.
---@param effect string The effect to check.
---@return boolean
function cc_check(effect)
    return cc_get(effect, false) ~= nil
end

--- Checks if the effect is currently active and marks the effect as acknowledged.
--- Make sure that the effect can actually be applied before calling this method.
---@param effect string The effect to check.
---@param response? string|table Optional response to send back to the server if the effect is active.
---@return boolean
function cc_ack(effect, response)
    for i, request in ipairs(cc_requests) do
        if request.code == effect then
            cc_start(request)
            if response then
                request.response = response
            end
            return true
        end
    end
    return false
end

--- Checks if the first supplied effect is active and the second is not.
---@param effect string The effect to check.
---@param unless string|string[] The effect(s) to check against.
---@param success_response? string|table Optional response to send back to the server if this check succeeds.
---@return boolean
function cc_ackunless(effect, unless, success_response)
    -- convert to table if necessary, else copy table
    if type(unless) == "string" then
        unless = {unless}
    else
        unless = unless and {unpack(unless)} or {}
    end
    -- remove `effect` from `unless` if present
    for i, e in ipairs(unless) do
        if e == effect then
            table.remove(unless, i)
            break
        end
    end
    -- check if `effect` is active and `unless` is not
    for i, request in ipairs(cc_requests) do
        if request.code == effect then
            cc_start(request)
            if success_response then
                request.response = success_response
            end
            return true
        else
            for j, e in ipairs(unless) do
                if request.code == e then
                    return false
                end
            end
        end
    end
    return false
end

--- Checks if the effect was active on the last frame.
--- Used for cleaning up effects that are no longer active.
---@param effect string|table The effect(s) to check.
---@return boolean
function cc_wasactive(effect)
	-- convert to table if necessary, else copy table
	if type(effect) == "string" then
		effect = {effect}
	else
		effect = effect and {unpack(effect)} or {}
	end
	-- ignore if currently active
	for j, e in ipairs(effect) do
		if cc_check(e) then
			return false
		end
	end
	-- do check
    for i, request in ipairs(old_requests) do
		for j, e in ipairs(effect) do
			if request.code == e then
				request.waschecked = true
				return true
			end
		end
    end
    return false
end

--- Checks if the Crowd Control server is currently active.
---@return boolean
function cc_isactive()
    return cc_thread and cc_thread:isRunning()
end

--- Initializes the Crowd Control server.
function cc_load()
    cc_thread = love.thread.newThread("crowdcontrol_thread.lua")
	cc_thread:start()
end

--- Reloads the Crowd Control server.
function cc_reload()
    if cc_isactive() then
        outgoing:push("close")
        cc_thread:wait()
    elseif cc_thread and cc_thread:getError() then
        print("Previous thread closed due to: " .. cc_thread:getError())
    end
    cc_load()
end

