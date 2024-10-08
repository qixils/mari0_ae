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
        local n = math.random(1, 58)
        local type = nil
        if n >= 55 then
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
        local n = math.random(1, 60)
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
