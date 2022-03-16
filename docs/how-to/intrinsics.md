# Intrinsics

## Generics

| Method | Arguments | Returns |
|---|---|---|
| `print` | `string message` | `null` |
| `wait` | `number time = 1` | `null` |
| `char` | `number charCode` | `string` |
| `code` | `string character` | `number` |
| `toString` | `any value` | `string` |
| `toNumber` | `any value` | `number` |
| `toInt` | `any value` | `number` |
| `toBoolean` | `any value` | `boolean` |
| `range` | `number from`, `number? to`, `number step = 1` | `list` |
| `rng` | `any? seed` | `number` |
| `abs` | `number value` | `number` |
| `acos` | `number value` | `number` |
| `asin` | `number value` | `number` |
| `atan` | `number value` | `number` |
| `ceil` | `number value` | `number` |
| `cos` | `number value` | `number` |
| `floor` | `number value` | `number` |
| `sin` | `number value` | `number` |
| `sign` | `number value` | `number` |
| `round` | `number value` | `number` |
| `sqrt` | `number value` | `number` |


| Variable | Value
|---|---|
| `pi` | `3.1415926535897931` |

## String intrinsics

| Method | Arguments | Returns |
|---|---|---|
| `trim` |  | `string` |
| `toUpperCase` |  | `string` |
| `toLowerCase` |  | `string` |
| `length` |  | `number` |
| `indexOf` | `string value`, `number offset = 0` | `number` |
| `lastIndexOf` | `string value`, `number? offset` | `number` |
| `contains` | `string value` | `boolean` |
| `replace` | `string value`, `string replaceWith = ""` | `string` |
| `split` | `string delimiter = ","` | `list` |
| `slice` | `number from = 0`, `number? to` | `string` |

## List intrinsics

| Method | Arguments | Returns |
|---|---|---|
| `hasIndex` | `number value` | `boolean` |
| `join` | `string delimiter = ","` | `string` |
| `push` | `any value` | `boolean` |
| `unshift` | `any value` | `boolean` |
| `shift` | | `any` |
| `pop` | | `any` |
| `removeAt` | `number index` | `boolean` |
| `length` | | `number` |
| `slice` | `number from = 0`, `number? to` | `list` |
| `values` | | `list` |
| `keys` | | `list` |
| `indexOf` | `any value`, `number offset = 0` | `number` |
| `lastIndexOf` | `any value`, `number? offset` | `number` |
| `find` | `function(item, index) fn` | `any` |
| `forEach` | `function(item, index) fn` | `null` |
| `map` | `function(item, index) fn` | `list` |
| `reduce` | `function(initialValue, item, index) fn`, `any initialValue = null` | `any` |

## Map intrinsics

| Method | Arguments | Returns |
|---|---|---|
| `hasIndex` | `string value` | `boolean` |
| `length` | | `number` |
| `delete` | `string key` | `boolean` |
| `values` | | `list` |
| `keys` | | `list` |
| `indexOf` | `any value` | `string` |
| `find` | `function(item, key) fn` | `any` |
| `forEach` | `function(item, key) fn` | `any` |