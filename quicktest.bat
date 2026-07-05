@echo off
echo NotifyIsland2 Quick Test
curl -s -X POST http://localhost:1379/api/notify -H "Content-Type: application/json" -d "{\"title\":\"NotifyIsland2\",\"title_duration\":3,\"content\":\"Test OK!\",\"content_duration\":8,\"sound_enabled\":true,\"effect_enabled\":true}"
echo.
pause
