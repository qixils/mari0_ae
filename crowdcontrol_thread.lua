local socket = require("socket")
local timer = require("love.timer")
local address, port = "localhost", 58435
local incoming = ""
local outgoing = love.thread.getChannel("cc_outgoing")
local requests = love.thread.getChannel("cc_requests")

local crowdcontrol = socket.tcp()
crowdcontrol:connect(address, port)
crowdcontrol:settimeout(0.1)

if not crowdcontrol or crowdcontrol:status() ~= "open" then
    print("Failed to connect to Crowd Control")
    return
end

while true do
    -- handle outgoing
    while outgoing:peek() do
        local msg = outgoing:pop()
        crowdcontrol:send(msg)
    end

    -- handle incoming
    local data, err = crowdcontrol:receive()
    if data then
        incoming = incoming .. data
        local messages = incoming:split("\0")
        for i, msg in ipairs(messages) do
            if i == #messages then break end
            if msg ~= "" then
                local request = JSON:decode(msg)
                if request ~= nil and request['type'] == 1 then -- if request to start
                    requests:push(request)
                end
            end
        end
        incoming = messages[#messages]
        timer.sleep(0.01)
    elseif err == "closed" then
        crowdcontrol:close()
        break
    end
end