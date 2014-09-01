# Chat

Chat is a simple, real-time Chat app showcasing ServiceStack's support for 
Server Events and Remote Control. More info at: https://github.com/ServiceStackApps/Chat

## Usage

Double-clicking Chat.exe will open Chat in your preferred browser. 
If `appsettings.txt` doesn't already exist, the default `appsettings.txt` will be exported and used.

appsettings.txt is an overridable config file that lets you customize how Chat is run:
 
 - `port` lets you change what port to run Chat.exe on
 - `redis` enables RedisServerEvents for fan-out events to support load-balanced app servers 
 - `oauth settings` specify OAuth provider settings to use, default uses ServiceStack's AuthWeb demo settings

```
# which port to host Chat on
port 1337

# uncomment below to enable fan-out RedisServerEvents via redis
#redis localhost
```

These options are also available from the command-line:

```
Usage: Chat.exe                        # Run using default appsettings.txt
Usage: Chat.exe /port=1337             # Run on port 1337
Usage: Chat.exe /redis=localhost:6379  # Run using RedisServerEvents
```

### Test Fan Out Redis Server Events

You run `test-fanout-redis-events.bat` script to launch multiple instances of chat hosted on different ports communicating via redis server events.

## Requirements

A .NET 4.5 runtime is required to run `Chat.exe`.
Linux/OSX platforms can install Mono: http://www.go-mono.com/mono-downloads/download.html

# About

The source code for Chat is available at: https://github.com/ServiceStack/ServiceStack.Gap/tree/master/src/Chat
An online preview hosted on IIS/ASP.NET is also available at: http://chat.servicestack.net
