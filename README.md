# Google Cloud Instance group updater

Provides tools for updating instances in Google Cloud Instance Groups.

## Process description

- Start template virtual machine if stopped.
  - Wait for start-up
- Run SSH command on instance.
  - Ask for confirmation before continuing unless pre-approved.
- Stop instance if continue was accepted.
  - Wait for instance to be stopped.
- Create Image from template instance root disk.
  - Wait for completion
- Create instance template based on default template with created image as instance root disk.
- Update Instance group with new instance template and rolling replace instances

``` C#
UpdatePolicy = new InstanceGroupManagerUpdatePolicy
                {
                    Type = "PROACTIVE",
                    MinimalAction = "REPLACE",
                    MaxSurge = new FixedOrPercent { Fixed__ = 3 },
                    MaxUnavailable = new FixedOrPercent { Fixed__ = 0 },
                }
```

## Projects

### Cli

Run update process one-by-one using FriendlyName

Usage example:

``` shell
Cli.exe -f <FriendlyName> -u <SshUsername> -k <Sshkey.pem>
```

### WebUI

Basic tool for managing entries in database.

### WorkerService (WIP)

Update configured instance groups on database scheduled.

### SharedLib

Shared code

## Requirements

- Authenticated gcloud sdk or service account file. Uses [Application Default Credentials](https://cloud.google.com/docs/authentication/provide-credentials-adc)
- .Net 7.x (When building)
- One of supported Database
  - MSSQL Server
  - MySQL (WIP)
  - Postgres
  - SQLite
- OS support
  - Windows 10+
  - Linux

## Building

### Release version for Linux

``` shell
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained=true
```

### Release version for Windows

``` shell
dotnet publish -c Release -r win10 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained=true
```
