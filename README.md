# DS4Indicator

**DS4Indicator** is a lightweight Windows desktop application designed to help users track the connection status of their DualShock 4 controllers and display important information about their usage. It provides real-time updates on whether your controller is connected or disconnected and offers easy access to the runtime since the last connection.

## Features

- **Controller Status**: Shows whether a controller is connected or disconnected.
- **Runtime Tracking**: Displays the time elapsed since the controller was last connected.
- **System Tray Integration**: A system tray icon that allows users to check controller status and access application settings.
- **Run at Startup**: An option to automatically launch DS4Indicator at system startup.
- **Battery Monitoring (In development)**: The app provides an estimation of the controller’s battery life, though it is important to note that the accuracy of the battery percentage is limited due to insufficient documentation on the DualShock 4's battery reporting.

## Accuracy of Battery Information

It is important to highlight that accurate battery percentage monitoring for the DualShock 4 controllers is not entirely feasible due to the limited documentation available for the DualShock 4. While **DS4Windows** provides an estimate of battery life, even its reported percentages are not fully accurate, as the controller itself does not provide a precise way to query the remaining battery level. As such, **DS4Indicator** should be considered to provide rough estimates of battery life and is not guaranteed to reflect the true battery percentage.

### Upcoming Features

- **Usage Time Warnings**: Future versions of **DS4Indicator** will include a feature to warn users when their controller has been in use for extended periods. This feature will allow users to monitor their controller usage and ensure the battery is charged accordingly to prevent unexpected disconnections.
  
## Installation

1. Download the latest release from the [Releases page](https://github.com/AneeshSharma9/DS4Indicator/releases).
2. Extract the files to a folder of your choice.
3. Launch the application (`DS4Indicator.exe`).

## Run at Startup

To make **DS4Indicator** launch automatically when your computer starts:

1. Open the system tray icon context menu.
2. Click on the **"Run at Startup"** option to enable or disable this feature.

## Disclaimer

Please note that while **DS4Indicator** provides an estimate of battery life, it is based on available data and the limitations of the DualShock 4 controller's hardware and documentation. The app cannot guarantee fully accurate battery readings. Use the information at your discretion and be mindful of charging your controller before it runs out of battery completely.
