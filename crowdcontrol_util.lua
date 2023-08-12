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
function cc_chack(effect)
    for i, request in ipairs(cc_requests) do
        if request.code == effect and not request.acknowledged then
            request.acknowledged = true
            return true
        end
    end
    return false
end