# DumpNetSession

## About

Simple program that dumps all sessions established on a server using [NetSessionEnum](https://docs.microsoft.com/en-us/windows/desktop/api/lmshare/nf-lmshare-netsessionenum)

Similar to `net.exe session \\computername`, but without the same access level requirement.

## Compile

Compile with csc.exe provided by .Net framework or some other C# compiler.

## Example

    C:\>C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe DumpNetSession.cs
    C:\>DumpNetSession.exe somehost.example.com
    cname: \\192.168.1.123, username: user1, time: 89984, idle_time: 89984
    cname: \\192.168.1.100, username: otheruser, time: 1075521, idle_time: 1075521
