cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
old_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

-- misc state variables
controls_inverted = false

function cc_send(msg)
    if type(msg) == "table" then
        msg = JSON:encode(msg)
    end
    outgoing:push(msg .. "\0")
end

-- Returns true if the effect is currently active and marks the effect as acknowledged.
-- Optionally saves a response to be sent back to the server.
function cc_ack(effect, response)
    for i, request in ipairs(cc_requests) do
        if request.code == effect then
            if not request.started then
                request.started = love.timer.getTime()
            end
            if response then
                request.response = response
            end
            return true
        end
    end
    return false
end

-- Returns true if the first supplied effect is active and the second is not.
-- Optionally may provide a list of effects to check.
-- Optionally saves a response to be sent back to the server upon success.
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
            request.started = love.timer.getTime()
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

-- Returns true if the effect was active on the last frame.
-- Used for cleaning up effects that are no longer active.
function cc_wasactive(effect)
    for i, request in ipairs(old_requests) do
        if request.code == effect then
            return true
        end
    end
    return false
end

function cc_isactive()
    return cc_thread and cc_thread:isRunning()
end

function cc_load()
    cc_thread = love.thread.newThread("crowdcontrol_thread.lua")
	cc_thread:start()
end

function cc_reload()
    if cc_isactive() then
        outgoing:push("close")
        cc_thread:wait()
    elseif cc_thread and cc_thread:getError() then
        print("Previous thread closed due to: " .. cc_thread:getError())
    end
    cc_load()
end