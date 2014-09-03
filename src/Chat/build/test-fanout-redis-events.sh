#!/bin/bash 

# Quick script to launch multiple versions of Chat hosted on different ports connected via redis server events
# Requires redis-server. To install redis see: http://redis.io/download

mono Chat.exe -port=1337 -redis=localhost & 
mono Chat.exe -port=2337 -redis=localhost -background="http://bit.ly/1oQqhtm" & 
mono Chat.exe -port=3337 -redis=localhost -background="http://bit.ly/1yIJOBH" & 
