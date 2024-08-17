@echo off
cd ../components
dotnet run ../tests/
cd ../tests/

gcc -c output.S -o output.o
gcc output.o -o output.exe
output
echo The program has returned: %errorlevel%
pause