@echo off
for %%I in ("%~dp0.") do set "REPO_DIR=%%~fI"

docker build -t blob-eat-blob-agent "%REPO_DIR%"
if errorlevel 1 exit /b %errorlevel%

docker run --rm -it --mount "type=bind,src=%REPO_DIR%,dst=/workspace" blob-eat-blob-agent bash -lc "bash /workspace/SetContainerRuntimeConfig.sh"