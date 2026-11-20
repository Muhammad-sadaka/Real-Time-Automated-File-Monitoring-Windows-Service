# 📂 File Monitoring Service — C# Windows Service

![Language](https://img.shields.io/badge/language-C%23-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![Framework](https://img.shields.io/badge/.NET%20Framework-4.x-purple)
![Type](https://img.shields.io/badge/type-Windows%20Service-green)

A Windows Service written in C# that watches a source folder, and whenever a new file appears it **renames the file with a unique GUID and moves it to a destination folder**. Every action is written to a log file.

It is a compact reference for:

- Building a Windows Service with `ServiceBase`
- Watching a folder with `FileSystemWatcher`
- Waiting until a file is fully written before touching it
- Reading settings from `App.config`
- Installing a service with `ProjInstaller` and `installutil`
- Running the same code as a **console app for debugging**

---

## ✨ Features

- Real-time monitoring of a folder (`FileSystemWatcher`, `Created` event)
- Safe file handling: waits until the file is not locked (up to 10 attempts, 500 ms apart)
- Renames each file to a new GUID and keeps the original extension
- Moves the file to a destination folder
- Timestamped logging to `ServiceLog.txt`
- Configurable folders through `App.config`, with built-in defaults
- Creates missing folders automatically
- Runs as a Windows Service (automatic start) or as a console app
- Echoes log messages to the console when run interactively

---

## 🧱 How It Works

```
Source folder ──(new file)──► wait until file is ready ──► rename to GUID ──► Destination folder
                                                                  │
                                                                  └──► ServiceLog.txt
```

1. `OnStart` creates a `FileSystemWatcher` on `SourceFolder`.
2. When a file is created, `OnFileCreated` checks `IsFileReady` by trying to open it with exclusive access.
3. If the file stays locked for more than 10 attempts, it is skipped and logged.
4. Otherwise it is moved to `DestinationFolder` as `{GUID}{original extension}`.
5. Success and errors are written to the log.

**Example**

```
Source:       invoice.pdf
Destination:  3f2a9c1e-7b4d-4c58-9d1a-5e6f8a2b0c11.pdf
```

---

## 📁 Project Structure

```
FileMonitoringService/
├── FileMonitoringService.cs   # Service logic (watcher, move, log)
├── ProjInstaller.cs           # Service installer configuration
├── Program.cs                 # Entry point
├── App.config                 # Folder settings
└── README.md
```

---

## ⚙️ Configuration

Edit `App.config`:

```xml
<configuration>
  <appSettings>
    <add key="SourceFolder" value="F:\FileMonitoring\Source" />
    <add key="DestinationFolder" value="F:\FileMonitoring\Destination" />
    <add key="LogFolder" value="F:\FileMonitoring\Logs" />
  </appSettings>
</configuration>
```

| Key                 | Description                       | Default                           |
|---------------------|-----------------------------------|-----------------------------------|
| `SourceFolder`      | Folder to watch                   | `F:\FileMonitoring\Source`        |
| `DestinationFolder` | Where renamed files are moved     | `F:\FileMonitoring\Destination`   |
| `LogFolder`         | Where `ServiceLog.txt` is written | `F:\FileMonitoring\Logs`          |

If a key is missing, the default is used and a warning is logged.

---

## 🚀 Getting Started

### Requirements
- Windows
- .NET Framework 4.x
- Visual Studio 2019+ (to build)
- Administrator rights (to install the service)

### 1. Clone and build
```bash
git clone https://github.com/<your-username>/<your-repo>.git
```
Open the solution in Visual Studio and build in **Release** mode.

### 2. Test in console mode (recommended first)
Run the project from Visual Studio. When started interactively, the service runs in the console and prints log messages. Press **Enter** to stop it.

### 3. Install the service
Open **Developer Command Prompt for VS as Administrator**:

```bash
installutil FileMonitoringService.exe
```

### 4. Start the service
```bash
net start FileMonitoringService
```

### 5. Stop and uninstall
```bash
net stop FileMonitoringService
installutil /u FileMonitoringService.exe
```

### Service settings
| Setting      | Value                                  |
|--------------|----------------------------------------|
| Service name | `FileMonitoringService`                |
| Display name | File Monitoring Service                |
| Start type   | Automatic                              |
| Account      | `LocalService`                         |
| Depends on   | `RpcSs`, `EventLog`, `LanmanWorkstation` |

> **Permissions:** the service runs as `LocalService`. Make sure this account has read/write access to the source, destination, and log folders, otherwise moves and logging will fail.

---

## 🧪 Quick Test

1. Start the service.
2. Copy any file into the source folder.
3. Within a second or two, the file disappears from the source and appears in the destination with a GUID name.
4. Check `ServiceLog.txt`:

```
[2026-11-20 03:10:41] File detected: F:\FileMonitoring\Source\report.docx
[2026-11-20 03:10:41] File moved: F:\FileMonitoring\Source\report.docx -> F:\FileMonitoring\Destination\9b1c...e4.docx
```

---

## ⚠️ Known Limitations

- Files that already exist in the source folder when the service starts are **not** processed (only new files trigger the event)
- Subfolders are not monitored (`IncludeSubdirectories = false`)
- The retry loop uses `Thread.Sleep`, which blocks the watcher thread while waiting
- Log writes are not synchronized, so many simultaneous files can cause write conflicts
- Only `IOException` is handled while checking readiness; permission errors surface in the generic catch
- Log folders are written to before they are guaranteed to exist if the configured folder is missing at construction time
- The original file name is only recorded in the log, so the log is the only way to map a GUID back to its original name

## 🗺️ Roadmap

- [ ] Process files already present at startup
- [ ] Thread-safe logging (lock or a logging library)
- [ ] Async/queued processing instead of `Thread.Sleep`
- [ ] Optional subfolder monitoring
- [ ] Log rotation by date or size
- [ ] Support for filters (extensions, size)
- [ ] Windows Event Log integration
- [ ] Unit tests

---

## 🤝 Contributing

1. Fork the repository
2. Create a branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push: `git push origin feature/your-feature`
5. Open a Pull Request

---

## 📄 License

Released under the [MIT License](LICENSE).

## 👤 Author

**Muhammad** — Junior Software Developer, Damascus, Syria

- GitHub: [@Muhammad-sadaka](https://github.com/Muhammad-sadaka)

If this project helped you learn, please give it a ⭐