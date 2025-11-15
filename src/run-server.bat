@echo off
cd /d "%~dp0TaskManager.Api"
start "TaskManager API" cmd /k "dotnet run & pause"