@echo off
echo ========================================
echo Запуск TaskManager API Server
echo ========================================
echo.

cd /d "%~dp0TaskManager.Api"

if not exist "TaskManager.Api.csproj" (
    echo Ошибка: Файл TaskManager.Api.csproj не найден!
    echo Убедитесь, что вы находитесь в правильной директории.
    pause
    exit /b 1
)

echo Проверка подключения к базе данных...
echo.

echo Запуск сервера...
echo API будет доступен по адресу: http://localhost:5233
echo Swagger UI: http://localhost:5233/swagger
echo.

dotnet run

pause