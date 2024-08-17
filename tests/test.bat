@echo off
cd ../components
dotnet run ../tests/
pause
cd ../tests/
cls
gcc -c output.S -o output.o
gcc output.o -o output.exe
output
echo The program has returned: %errorlevel%
pause