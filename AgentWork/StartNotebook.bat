@echo off
cd /d "%~dp0"

if not exist .venv python -m venv .venv
call .venv\Scripts\activate.bat
.venv\Scripts\python.exe -m pip install --upgrade -r requirements.txt
.venv\Scripts\python.exe -m notebook
