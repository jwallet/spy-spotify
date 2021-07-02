@ECHO OFF
ECHO Reset Spytify to its default settings.
ECHO If you continue, it will remove all user settings from any Spytify current or older versions on your system.
PAUSE
DEL /S /q "%userprofile%\AppData\Local\Spytify\*"
PAUSE
