@echo off
cd ../components
dotnet run ../tests/ entry.c
cd ../tests/

gcc -o output.exe output.S
output.exe
echo The program has returned: %errorlevel%
pause