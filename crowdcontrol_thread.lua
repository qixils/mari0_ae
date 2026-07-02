local socket = require("socket")
local timer = require("love.timer")
local json = require("JSON")
local address, port = "localhost", 28379
local outgoing = love.thread.getChannel("cc_outgoing")
local requests = love.thread.getChannel("cc_requests")
local status = love.thread.getChannel("cc_status")

local function setstatus(s)
    status:clear()
    status:push(s)
end

--- Connects to the Crowd Control server, retrying until it succeeds.
--- Returns nil if a "close" message arrives while waiting.
local function connect()
    setstatus("connecting")
    while true do
        local sock = socket.tcp()
        sock:settimeout(1)
        local success, err = sock:connect(address, port)
        if success then
            sock:settimeout(0) -- everything past this point must be non-blocking
            setstatus("connected")
            return sock
        end
        sock:close()
        print("Failed to connect to Crowd Control: " .. tostring(err) .. " (retrying in 5 seconds)")
        -- Wait before retrying; discard stale outgoing messages but honor "close"
        for _ = 1, 50 do
            while outgoing:peek() do
                if outgoing:pop() == "close" then
                    print("Aborting Crowd Control connection per util request")
                    return nil
                end
            end
            timer.sleep(0.1)
        end
    end
end

local crowdcontrol = connect()
if not crowdcontrol then return end

local incoming = ""
local sendbuf = ""

local function disconnected()
    print("Crowd Control connection closed")
    setstatus("disconnected")
    crowdcontrol:close()
    requests:push("unknown_error")
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
        print("Sending message", msg)
        sendbuf = sendbuf .. msg
    end
    -- flush the send buffer, keeping whatever the socket didn't accept
    -- (a partial send that gets dropped would corrupt the framing of every message after it)
    if sendbuf ~= "" then
        local sent, err, lastbyte = crowdcontrol:send(sendbuf)
        if sent then
            sendbuf = ""
        elseif err == "closed" then
            disconnected()
            return
        else
            sendbuf = sendbuf:sub((lastbyte or 0) + 1)
        end
    end

    -- handle incoming ("*a" with a zero timeout returns everything buffered as the partial result)
    local data, err, partial = crowdcontrol:receive("*a")
    data = data or partial
    if data and data ~= "" then incoming = incoming .. data end
    -- parse every complete null-terminated message, not just the first
    while true do
        local null = incoming:find("\0", 1, true)
        if not null then break end
        local message = incoming:sub(1, null - 1)
        incoming = incoming:sub(null + 1)
        print("Received message", message)
        pcall(function()
            local request = json:decode(message)
            if request ~= nil then
                requests:push(request)
            end
        end)
    end
    if err == "closed" then
        disconnected()
        return
    end
    timer.sleep(0.01)
end
