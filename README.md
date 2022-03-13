# PseudoScript

Scripting language highly inspired by [Greybel](https://github.com/ayecue/greybel-js) and [MiniScript](https://github.com/JoeStrout/miniscript).

The main purpose of PseudoScript is to create a scripting language which is easy to use and easy to extend.

## Features

- synchronous to enable easily understandable usage, interpreter itself will return a task though to not block the main thread
- includes debugger feature which can be used to create your own debugger
- types included are string, number, boolean, null, map and list
- basic intrinsics for strings, lists and maps
- easy syntax
- dynamic imports which can be managed via a ResourceHandler which enables you to include files how you want, by default it will use System.IO.File
- pausing and resuming script execution
- inject code into a running script
- api and intrinsics can be integrated and extended very easily

## TODO

- integrate proper documentation
- possibly more intrinsics

For now you can take a look at the tests to get an idea of the syntax and usage. A proper documentation going to be added at a later point.