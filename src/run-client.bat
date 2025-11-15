@echo off
cd /d "%~dp0TaskManager"
start "TaskManager" cmd /k "dotnet run & pause"