local socket = require("socket")
local timer = require("love.timer")
local json = require("json")
local address, port = "localhost", 58435
local incoming = ""
local outgoing = love.thread.getChannel("cc_outgoing")
local requests = love.thread.getChannel("cc_requests")

local crowdcontrol = socket.tcp()
crowdcontrol:settimeout(0.1)
local success, error = crowdcontrol:connect(address, port)

if error then
    print("Failed to connect to Crowd Control: " .. error)
    return
end

while true do
    -- handle outgoing
    while outgoing:peek() do
        local msg = outgoing:pop()
        if msg == "close" then
            print("Aborting Crowd Control connection per util request")
            crowdcontrol:close()
            return
        end
        crowdcontrol:send(msg)
    end

    -- handle incoming
    local data, err, partial = crowdcontrol:receive()
    data = (err and partial) or data
    if data then incoming = incoming .. data end
    if incoming ~= "" then
        local null = incoming:find("\0")
        if null then
            local request = json:decode(incoming:sub(1, null-1))
            if request ~= nil and request['type'] == 1 then -- if request to start
                requests:push(request)
            end
            incoming = incoming:sub(null+1)
        end
        timer.sleep(0.01)
    end
    if err == "closed" then
        print("Crowd Control connection closed")
        crowdcontrol:close()
        return
    end
end