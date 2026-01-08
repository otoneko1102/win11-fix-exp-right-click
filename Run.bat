@echo off
cd /d %~dp0
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0Win11FixExpRightClick.ps1'"

if %errorlevel% neq 0 (
    echo.
    echo ---------------------------------------------------
    echo エラーが発生しました。ウィンドウを閉じずに表示しています。
    echo Error occurred. Press any key to close.
    echo ---------------------------------------------------
    pause
)
