REM Quick script to launch multiple versions of Chat hosted on different ports connected via redis server events
REM Requires redis-server. To install redis on Windows see: https://github.com/ServiceStack/redis-windows

START Chat.exe /port=1337 /redis=localhost
START Chat.exe /port=2337 /redis=localhost /background=http://bit.ly/1oQqhtm
START Chat.exe /port=3337 /redis=localhost /background=http://bit.ly/1yIJOBH