cc_request_channel = love.thread.getChannel("cc_requests")
cc_requests = {}
local outgoing = love.thread.getChannel("cc_outgoing")

function cc_send(msg)
    if type(msg) == "table" then
        msg = JSON:encode(msg)
    end
    outgoing:push(msg .. "\0")
end

function cc_isactive(effect)
    for _, request in ipairs(cc_requests) do
        if request.code == effect then
            return true
        end
    end
    return false
end