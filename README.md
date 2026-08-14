# 📂 Real-Time File Monitoring & Processing Windows Service

**File Monitoring Service** is an automated background Windows Service developed using **C# (.NET Framework)** to continuously monitor specified source directories for incoming files in real-time[span_0](start_span)[span_0](end_span). Built for background automation, the system detects newly created files, validates file lock readiness, renames files dynamically using unique GUIDs to prevent naming collisions, transfers them to a target destination folder, and maintains an operational execution log[span_1](start_span)[span_1](end_span).

---

## 🌟 Key Features

### 📁 Real-Time Directory Monitoring
* **Event-Driven File Watcher:** Utilizes `FileSystemWatcher` to detect newly added files instantly without polling performance overhead[span_2](start_span)[span_2](end_span).
* **GUID-Based Renaming:** Automatically renames incoming files with globally unique identifiers (`Guid.NewGuid()`) prior to transfer to ensure data integrity and avoid overwriting existing files[span_3](start_span)[span_3](end_span).

### 🔒 File Access & Lock Validation
* **I/O Readiness Check:** Features an iterative lock-checking mechanism (`IsFileReady`) that verifies files are fully written and released by external processes before attempting operations[span_4](start_span)[span_4](end_span).
* **Retry Safety Handling:** Retries file availability checks in 500ms intervals (up to 10 attempts) with dynamic timeout handling[span_5](start_span)[span_5](end_span).

### 🔄 Dual Execution Modes
* **Background Windows Service:** Operates silently in the system background managed by Service Control Manager (SCM) under `LocalService` identity[span_6](start_span)[span_6](end_span).
* **Interactive Console Mode:** Supports direct console startup via `Environment.UserInteractive` for easy testing, debugging, and live output monitoring[span_7](start_span)[span_7](end_span).

### 📝 Logging & Auto-Provisioning
* **Comprehensive Audit Trail:** Logs all detection events, successful file transfers, lock delays, and runtime exceptions into `ServiceLog.txt`[span_8](start_span)[span_8](end_span).
* **Automatic Folder Creation:** Automatically provisions source, destination, and log directories upon startup if missing[span_9](start_span)[span_9](end_span).

### ⚙️ System Dependencies & Stability
* **Service Dependencies:** Configured with native Windows service dependencies (`RpcSs`, `EventLog`, `LanmanWorkstation`) to guarantee network and system readiness[span_10](start_span)[span_10](end_span).

---

## 🛠️ Tech Stack & Architecture

* **Language:** C#[span_11](start_span)[span_11](end_span)
* **Framework:** .NET Framework (`System.ServiceProcess`, `System.Configuration.Install`)[span_12](start_span)[span_12](end_span)
* **Core Components:** `FileSystemWatcher`, `System.IO` (FileStream / Directory Operations), `System.Threading`[span_13](start_span)[span_13](end_span)

---

## ⚙️ Configuration (App.config)

All folder paths are fully configurable without modifying or rebuilding code[span_14](start_span)[span_14](end_span):

<appSettings>
  <add key="SourceFolder" value="F:\FileMonitoring\Source" />
  <add key="DestinationFolder" value="F:\FileMonitoring\Destination" />
  <add key="LogFolder" value="F:\FileMonitoring\Logs" />
</appSettings>

---

## 🚀 How to Run & Install

### Prerequisites
* **Visual Studio** (2019 / 2022 / 2026) with .NET Desktop Development workload.

---

### Option 1: Running in Debug / Console Mode
Simply run the compiled executable (`FileMonitoringService.exe`) directly[span_15](start_span)[span_15](end_span). The application detects interactive mode and opens a console window for real-time monitoring[span_16](start_span)[span_16](end_span).

---

### Option 2: Installing as a Native Windows Service

1. **Open Developer Command Prompt for Visual Studio** as **Administrator**.
2. **Navigate to the output build directory** containing `FileMonitoringService.exe`.
3. **Register the Service:**
   InstallUtil.exe FileMonitoringService.exe
4. **Manage Service:**
   * Open `services.msc` on your machine.
   * Locate **File Monitoring Service**[span_17](start_span)[span_17](end_span).
   * Start or set startup type to **Automatic**[span_18](start_span)[span_18](end_span).

5. **Uninstall Service:**
   InstallUtil.exe /u FileMonitoringService.exe

---

## 👨‍💻 Author

**Muhammad Sadaka**  
* GitHub: [@Muhammad-sadaka](https://github.com/Muhammad-sadaka)

