<br/><br/>

<p align="center">
 <a href=#><img src=".github/RESOURCES/logo.png"></a>
</p>
<p align="center"><b>WWU Learnweb Notifier Service</b></p>

<br/>

<hr />
<p align="center">
    <a href="https://github.com/passlickdev/learnweb-notifier/releases"><b>Download Binaries</b></a>&nbsp;&nbsp;–&nbsp;
    <a href="#technology">Technology</a>&nbsp;&nbsp;–&nbsp;
    <a href="#setup--installation">Setup & Installation</a>&nbsp;&nbsp;–&nbsp;
    <a href="#licensing">Licensing</a>&nbsp;&nbsp;–&nbsp;
    <a href="https://passlickdev.com/redirect?id=4002">Donate</a>
</p>
<hr />
<br/>

The <b>Learnweb Notifier Service</b> monitors your Learnweb courses and notifies you when new content is available. The service is compatible with [Learnweb](https://www.uni-muenster.de/LearnWeb/learnweb2/) of the WWU University of Münster, which is based on [Moodle](https://moodle.org/). Completely written in C# on the platform-independent .NET Core environment. Developed by Passlick Development.  

<br/>

## Technology

### General
Learnweb Notifier retrieves your Learnweb courses regularly and notifies you as soon as there is a new activity in the course, which eliminates tedious manual retrieval of courses for new content. The service runs as a server app on all common operating systems and can be deployed manually or as a Docker container. To run the service, a Learnweb SSO account of the University of Münster is required.

### Modules

The service is divided into separate modules. The purpose of the individual modules is explained below.

#### LearnwebNotifier.Library
The library is the core of the service. It provides basic functionality for the service, like the retrieval of Learnweb courses or the parsing of activities.

#### LearnwebNotifier.Test
This module provides unit tests and test resources for the service and can be used to test the service.

#### LearnwebNotifier.Push
This module implements the library and provides a universal worker service that regularly checks courses and sends push notifications using Pushover.

### Requirements for Courses
To retrieve the latest activities of a course, the "Recent Activities" widget is used, which is usually embedded in the course page. In order for the service to be able to retrieve activities for a course, the widget must be embedded in the course page, otherwise the service will not work for the course!

### Client Notifications (Push)
In this current version, notifications of new activities are sent to your mobile phone as push notifications. This is currently implemented by [Pushover](https://pushover.net/), which is a push service for custom push messages on iOS, Android and desktop. To receive notifications in the current version, a pushover account and the correspondingly installed app is required. Please note that the Pushover service requires a one-time license payment of $5.00. A separate client for receiving and managing activities is planned for future versions of the Learnweb Notifier Service.

### Course Activities
The service retrieves activities from courses and notifies you of new activities. Below you will find the types of activities currently supported.

#### Supported Activities
Every first-level activity in courses, for example ...
* New/updated file in course
* New/updated link in course
* New/updated assignment in course
* Deletion of files/folders/...
* ...

#### Non-Supported Activities
Second-level activities, for example ...
* Activities inside folders
* Assignment upload confirmations
* Forum posts and updates (covered by Learnweb notifications)
* ...

## Setup & Installation (Push)
> Note: Since the service is mainly developed for Linux as a target system, the following installation instructions and software binaries are only available for Linux (using Debian 9 as an example) at this time

### System Requirements
* OS: Linux (e.g. Debian)  
The service can run on virtually any host. For example, you can use a vServer instance or a Raspberry Pi!
* RAM: ~50 MB
* Environment: .NET Core 3.1 runtime, Docker (optional)  



### Setup Pushover
1. Go to [Pushover](https://pushover.net/) and create an account, if not already existing
2. You have to register an application to receive notifications; just follow [this documentation](https://pushover.net/api#registration) to do so
3. After you created an acount and registered an application, you will receive an `User Key` and `Application API Token`, which will be used later on
4. Download the Pushover App on your desired devices and log-in with your account  

### Install with Docker
> Support for Docker coming soon! Please install manually

### Manual Installation
We recommend installing the service with Docker, but the service can also be installed manually. Just follow the steps below.

#### Install .NET Core
The service requires .NET Core 3.1 runtime. Refer to [this documentation](https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-debian9) to learn how to install .NET Core on your system. Below is an example of installing the runtime on Debian 9. Execute the commands in the given order to install the runtime.

```bash
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-runtime-3.1
```

#### Copy Binaries

Head over to the [Releases](https://github.com/passlickdev/learnweb-notifier/releases) and download the latest compiled version. Copy the whole content to a folder of your choice (e.g. your home directory) on your host system and make sure to do this as a non-root user.

#### Run the Service

Change into the directory in which you copied the binaries and run ``dotnet lwnotif-push.dll``. Follow the steps of the Configuration Assistant at the first start of the service to create a config file. After the setup routine, restart the service with the previous command. You are good to go!

It is recommended to run the service in a separate [screen](https://linuxize.com/post/how-to-use-linux-screen/) session. You can also set up the service as a systemd daemon after creating a config file.





## Licensing

Copyright (c) 2020 Passlick Development. All rights reserved. 

Licensed under the **GNU General Public License v3.0** (the "License"); you may not use this file except in compliance with the License. 

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the [LICENSE](./LICENSE) for the specific language governing permissions and limitations under the License.

This project is neither developed by the University of Münster, nor is it related to it. The software uses inofficial ways to retrieve the required data, which could lead to account suspensions. The software is used at your own risk!


<br/>
<hr />
<br/><br/>
<p align="center">
 <a href="https://passlickdev.com/en/"><img src=".github/RESOURCES/passlickdev.png"></a>
</p>
<br/>