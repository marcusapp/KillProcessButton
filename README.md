# KillProcessButton

**KillProcessButton** is a button at the top right corner on the screen. Usually for some full screen program to exit itself especially in a kiosk. Built with **C#** and **.NET Framework 4.0**. 

----------

## 📋 System Requirements

-   **Operating System**: Windows XP or later (compatible with .NET Framework 4.0).
-   **Storage**: At least 10 MB of free space.

----------

## 🚀 Installation Guide

### Step 1: Download KillProcessButton

1.  Download the latest `KillProcessButton.exe` executable from [GitHub Releases](https://github.com/marcusapp/KillProcessButton/releases).
2.  Just place it on anywhere you like as a portable exe.

### Step 2: Config INI

1.  First time to run `KillProcessButton.exe` will create a `KillProcessButton.config.ini` in the same directory.
2.  Edit `KillProcessButton.config.ini`, change the value of key 'ProcessName' as your process name in Task Manager >> Detail. Only the process name, no need .exe at the tail.
   
```
[General]
ProcessName=notepad
ForceKillProcess=True
ButtonWidth=32
ButtonHeight=32
ButtonBackColor=#FF0000
ButtonFrontColor=#FFFFFF
PopupConfirm=True
PopupConfirmTitle=Confirmation
PopupConfirmMessage=Are you sure you want to close the application?
```

### Step 3: Run the Program

1.  Double-click `KillProcessButton.exe` to launch the application.
2.  If the target process running in background, a close button will show at the top right of screen.

----------

## 🎵 Usage Instructions

1.  **Launch the Program**:
    
    -   Run `KillProcessButton.exe`, program will auto detect the background process.
2.  **Exit the process**:
    -   Click the button to exit the target process.
    -   If the process exit successfully, top-right button will be hidden but program still running in background and monitoring until the process running again.
3.  **Exit the Program**:
    -   Right click the tray icon then choose [Exit] on the tray menu.

----------

## 📂 Folder Structure

Below is an example of the standard folder structure for NoSleepAudio:

```
KillProcessButton/
├── KillProcessButton.exe
├── KillProcessButton.config.ini

```

-   **KillProcessButton.exe**: The main executable file.
-   **KillProcessButton.config.ini**: The config file.

----------

## 🛠️ Developer Information

NoSleepAudio is developed using **C#** and **.NET Framework 4.0**, leveraging Windows' native audio playback capabilities for high performance and stability. Key technical details include:

-   **Programming Language**: C#
-   **Framework**: .NET Framework 4.0

----------
