cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

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
            request.started = love.timer.getTime()
            if response then
                request.response = response
            end
            return true
        end
    end
    return false
end

function cc_isactive()
    return cc_thread and cc_thread:isRunning()
end

function cc_load()
    cc_thread = love.thread.newThread("crowdcontrol.lua")
	cc_thread:start()
end

function cc_reload()
    if cc_isactive() then
        cc_request_channel:push("close")
        cc_thread:wait()
    end
    cc_load()
end