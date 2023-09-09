cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
old_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

-- misc state variables
wasd_inverted = false
buttons_inverted = false
toggled_lightsout = false

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
---@param effect string The effect to check.
---@return boolean
function cc_wasactive(effect)
    for i, request in ipairs(old_requests) do
        if request.code == effect then
            return true
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