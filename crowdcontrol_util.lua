cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

function cc_send(msg)
    if type(msg) == "table" then
        msg = JSON:encode(msg)
    end
    outgoing:push(msg .. "\0")
end

-- Portmaneau of "check" and "acknowledge", this returns true if the effect is
-- active and marks the effect as acknowledged.
function cc_chack(effect, response)
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