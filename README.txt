MoE-Sandbox (https://github.com/alvin-nt/moe-sandbox)

Sandbox for Windows, implemented in C#. Inspired by AppStract (https://code.google.com/archive/p/appstract/)

--------------------------------------------------------------------

Untuk menjalankan aplikasi dalam arsip ini, dibutuhkan hal-hal sebagai berikut:
1. Visual Studio 2015
2. Koneksi internet, untuk mengunduh library via NuGet.
3. Akun administrator

Aplikasi ini telah diuji dalam Windows 10 x64 (Anniversary Update).

Visual Studio Project yang disertakan dalam arsip ini:
1. HookLibraryRunner (injector untuk HookLibrary)
2. HookLibrary (library untuk intercept ke ntdll.dll)
3. HookTestRunner (injector untuk HookTest)
4. HookTest (library untuk intercept ke kernel32.dll)
5. SimpleSandboxShell (aplikasi untuk membatasi shell, ala SEB; belum diimplementasikan)
6. SimpleSandboxService (Windows Service yang terkait dengan SimpleSandboxShell; belum diimplementasikan)

Sejauh ini, sandbox hanya mampu melakukan logging system call pada aplikasi. Selain itu, injector (HookLibraryRunner/HookTestRunner) hanya dapat melakukan injeksi setelah aplikasi dijalankan.

--------------------------------------------------------------------

The following stuff are required in order to run the application in this archive:
1. Visual Studio 2015
2. Internet connection, for downloading libraries via NuGet.
3. Administrator account

Visual Studio Projects included in this archive:
1. HookLibraryRunner (injector for HookLibrary)
2. HookLibrary (library for intercepting ntdll.dll)
3. HookTestRunner (injector for HookTest)
4. HookTest (library for intercepting kernel32.dll)
5. SimpleSandboxShell (shell sandbox, ala SEB; not yet implemented)
6. SimpleSandboxService (Windows Service for SimpleSandboxShell; not yet implemented)

This application has been tested on Windows 10 x64 (Anniversary Update).

Currently, the sandbox is only capable of logging system calls used by an app. Moreover, the injector (HookLibraryRunner/HookTestRunner) can only inject the intercept library after an app has been run.