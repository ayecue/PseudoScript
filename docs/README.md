# PseudoScript

[![PseudoScript](https://circleci.com/gh/ayecue/PseudoScript.svg?style=svg)](https://circleci.com/gh/ayecue/PseudoScript)

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

## Documentation

[Github Page](https://ayecue.github.io/PseudoScript)

```
npm i docsify-cli -g
docsify serve docs
```

## TODO

- improve docs
