@ECHO OFF
SETLOCAL EnableDelayedExpansion
CLS

SET "buildVersion="
IF DEFINED GITHUB_RUN_NUMBER (
	SET buildVersion=%GITHUB_RUN_NUMBER%
	SET buildVersion=00000!buildVersion!
	SET buildVersion=!buildVersion:~-6!
	SET buildVersion=build!buildVersion!
) ELSE (
	SET /p versionSuffix="Please enter a version suffix (e.g. alpha001):"
	SET buildVersion=!versionSuffix!
)

SET "target=%*"
IF NOT DEFINED target (
	SET "target=default"
)

dotnet tool install fake-cli --tool-path tools\bin\ --version 5.*
tools\bin\fake.exe run tools\build.fsx --parallel 3 --target %target% -e buildVersion=!buildVersion!
