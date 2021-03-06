# Mittosoft.DnsServiceDiscovery

A pure .Net managed API for interfacing to Apple/Bonjour's mDNSResponder service on Windows, to enable DNS Service Discovery.  This library bypasses the several layers of indirection provided in the Bonjour SDK, including the COM Interop layers, and is a 100% TAP (Task-based Asynchronous Pattern) implementation.

The library currently implements the following operations:
- Browse Services
- Register Services
- Resolve Services
- Lookup Host Address Information

You can download the [Nuget package at this link](https://www.nuget.org/packages/Mittosoft.DnsServiceDiscovery)

System Requirements:
This library depends on the mDNSResponder service included with the [Bonjour SDK for Windows](https://download.developer.apple.com/Developer_Tools/bonjour_sdk_for_windows_v3.0/bonjoursdksetup.exe).  The library was tested with version 3.0 of the SDK.

At some point I would like to test the library with other mDNSResponder builds running on other platforms, and with the Avahi Bonjour compatibility library on Linux.

I will be adding new features in the future as time permits.

Currently only building as a .Net Standard 2.0 library.  I will add new build targets in the future if there is demand for it.

For library usage, have a look at the ConsoleApp and ServiceBrowser projects in the repo.  I will add documentation (comments) at some point in the future.

I have a fairly long TODO list associated with this project and will be updating the project in the future as time permits.

I will be adding Github-related content including CONTRIBUTING guidelines.

This is my first open-source contribution and I'm by no means a Git/Github guru, so please bear with me as I become more familiar with the community and tools.

[![Build Status](https://dev.azure.com/steveheckel/steveheckel/_apis/build/status/SteveHeckel.DnsServiceDiscovery?branchName=master)](https://dev.azure.com/steveheckel/steveheckel/_build/latest?definitionId=1&branchName=master)
