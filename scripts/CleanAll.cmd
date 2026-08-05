@echo off
echo Cleaning files...
for /d /r ".\..\" %%d in (bin,obj,.vs,node_modules) do @if exist "%%d" rd /s/q "%%d"
echo Terminated
pause