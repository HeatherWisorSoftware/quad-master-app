@echo off
echo Starting Blazor Demo...
echo.
echo Starting the server...
echo Press Ctrl+C to stop the demo.
echo.

start /B QuadMasterApp.exe --DatabaseOptions:ResetOnStartup=true

echo Waiting for server to start...
timeout /t 5 /nobreak > nul

echo Opening browser...
start http://127.0.0.1:5000

echo.
echo Server is running. Close this window to stop the app.
pause