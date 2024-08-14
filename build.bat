@echo off
dotnet run
gcc -c test.S -o test.o
gcc test.o -o run.exe
run
echo program returned: %errorlevel% 
pause