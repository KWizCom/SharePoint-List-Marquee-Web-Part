@ECHO OFF
ECHO Installing Web Part...

IF EXIST "C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\BIN\stsadm.exe" "C:\Program Files\Common Files\Microsoft Shared\web server extensions\60\BIN\stsadm" -o addwppack -filename "ListMarquee_Deploy.CAB" -globalinstall -force

IF EXIST "C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm.exe" "C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm" -o addwppack -filename "ListMarquee_Deploy.CAB" -globalinstall -force

recycle.js

pause