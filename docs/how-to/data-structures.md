# Data Structures

PseudoScript as of right now features following data structures by default:

- `String`
- `Number`
- `Boolean`
- `List`
- `Map`
- `Null`

**String**, **List** and **Map** contain certain default intrinsics which make it easier to work with them. Take a look [here](/how-to/intrinsics) to get a better insight.

## String

| Operator | Code | Result |
|---|---|---|
| <center>+</center> | <code>str = "test"<br>&nbsp;str = str + 12<code> | `str == "test12"` |
| <center>+=</center> | <code>str = "test"<br>&nbsp;str += 12<code> | `str == "test12"` |
| <center>[x]</center> | <code>str = "test"<br>&nbsp;strChar = str[1]<code> | `strChar == "e"` |
| <center>[-x]</center> | <code>str = "test"<br>&nbsp;strChar = str[-2]<code> | `strChar == "s"` |
| <center>==</center> | <code>result = "test" == "test"<code> | `result == true` |
| <center>!=</center> | <code>result = "test" != "other"<code> | `result == true` |
| <center><</center> | <code>result = "test" < "other"<code> | `result == true` |
| <center>></center> | <code>result = "test" > "other"<code> | `result == false` |
| <center><=</center> | <code>result = "test" <= "other"<code> | `result == true` |
| <center>>=</center> | <code>result = "test" >= "other"<code> | `result == false` |
| <center>and</center> | <code>result = "test" and "other"<code> | `result == true` |
| <center>or</center> | <code>result = "" or "other"<code> | `result == true` |

## Number

| Operator | Code | Result |
|---|---|---|
| <center>+</center> | <code>number = 10<br>&nbsp;number = number + 2<code> | `number == 12` |
| <center>+</center> | <code>number = 10<br>&nbsp;number = number + "10"<code> | `number == 20` |
| <center>+</center> | <code>number = 10<br>&nbsp;number = number + "foo"<code> | `number == 10` |
| <center>+=</center> | <code>number = 10<br>&nbsp;number += 2<code> | `number == 12` |
| <center>-</center> | <code>number = 10<br>&nbsp;number = number - 2<code> | `number == 8` |
| <center>-=</center> | <code>number = 10<br>&nbsp;number -= 2<code> | `number == 8` |
| <center>*</center> | <code>number = 10<br>&nbsp;number = number * 2<code> | `number == 20` |
| <center>*=</center> | <code>number = 10<br>&nbsp;number *= 2<code> | `number == 20` |
| <center>/</center> | <code>number = 10<br>&nbsp;number = number / 2<code> | `number == 5` |
| <center>/=</center> | <code>number = 10<br>&nbsp;number /= 2<code> | `number == 5` |
| <center>%</center> | <code>number = 10<br>&nbsp;number = number % 2<code> | `number == 0` |
| <center>^</center> | <code>number = 10<br>&nbsp;number = number ^ 2<code> | `number == 8` |
| <center><<</center> | <code>number = 10<br>&nbsp;number = number << 2<code> | `number == 40` |
| <center>>></center> | <code>number = 10<br>&nbsp;number = number >> 2<code> | `number == 2` |
| <center>>>></center> | <code>number = 10<br>&nbsp;number = number >>> 2<code> | `number == 2` |
| <center>&#124;</center> | <code>number = 10<br>&nbsp;number = number &#124; 2<code> | `number == 10` |
| <center>&</center> | <code>number = 10<br>&nbsp;number = number & 2<code> | `number == 2` |
| <center>==</center> | <code>result = 10 == 10<code> | `result == true` |
| <center>!=</center> | <code>result = 10 != 11<code> | `result == true` |
| <center><</center> | <code>result = 10 < 11<code> | `result == true` |
| <center>></center> | <code>result = 10 > 11<code> | `result == false` |
| <center><=</center> | <code>result = 10 <= 11<code> | `result == true` |
| <center>>=</center> | <code>result = 10 >= 11<code> | `result == false` |
| <center>and</center> | <code>result = 10 and 11<code> | `result == true` |
| <center>or</center> | <code>result = 0 or 10<code> | `result == true` |

## Boolean

| Operator | Code | Result |
|---|---|---|
| <center>+</center> | <code>boolean = true<br>&nbsp;boolean = boolean + 2<code> | `boolean == 3` |
| <center>+</center> | <code>boolean = true<br>&nbsp;boolean = boolean + "10"<code> | `boolean == 12` |
| <center>+</center> | <code>boolean = true<br>&nbsp;boolean = boolean + "foo"<code> | `boolean == 1` |
| <center>+=</center> | <code>boolean = true<br>&nbsp;boolean += 2<code> | `boolean == 3` |
| <center>-</center> | <code>boolean = true<br>&nbsp;boolean = boolean - 2<code> | `boolean == -1` |
| <center>-=</center> | <code>boolean = true<br>&nbsp;boolean -= 2<code> | `boolean == -1` |
| <center>*</center> | <code>boolean = true<br>&nbsp;boolean = boolean * 2<code> | `boolean == 2` |
| <center>*=</center> | <code>boolean = true<br>&nbsp;boolean *= 2<code> | `boolean == 2` |
| <center>/</center> | <code>boolean = true<br>&nbsp;boolean = boolean / 2<code> | `boolean == 0.5` |
| <center>/=</center> | <code>boolean = true<br>&nbsp;boolean /= 2<code> | `boolean == 0.5` |
| <center>%</center> | <code>boolean = true<br>&nbsp;boolean = boolean % 2<code> | `boolean == 1` |
| <center>^</center> | <code>boolean = true<br>&nbsp;boolean = boolean ^ 2<code> | `boolean == 3` |
| <center><<</center> | <code>boolean = true<br>&nbsp;boolean = boolean << 2<code> | `boolean == 4` |
| <center>>></center> | <code>boolean = true<br>&nbsp;boolean = boolean >> 2<code> | `boolean == 0` |
| <center>>>></center> | <code>boolean = true<br>&nbsp;boolean = boolean >>> 2<code> | `boolean == 0` |
| <center>&#124;</center> | <code>boolean = true<br>&nbsp;boolean = boolean &#124; 2<code> | `boolean == 3` |
| <center>&</center> | <code>boolean = true<br>&nbsp;boolean = boolean & 2<code> | `boolean == 0` |
| <center>==</center> | <code>result = true == true<code> | `result == true` |
| <center>!=</center> | <code>result = true != false<code> | `result == true` |
| <center><</center> | <code>result = true < false<code> | `result == true` |
| <center>></center> | <code>result = true > false<code> | `result == false` |
| <center><=</center> | <code>result = true <= false<code> | `result == true` |
| <center>>=</center> | <code>result = true >= false<code> | `result == false` |
| <center>and</center> | <code>result = true and true<code> | `result == true` |
| <center>or</center> | <code>result = false or true<code> | `result == true` |

## List

| Operator | Code | Result |
|---|---|---|
| <center>+</center> | <code>list = ["foo", "bar"]<br>&nbsp;list = list + ["moo"]<code> | `list == ["foo", "bar", "moo"]` |
| <center>[x]</center> | <code>list = ["foo", "bar"]<br>&nbsp;item = list[0]<code> | `item == "foo"` |
| <center>[-x]</center> | <code>list = ["foo", "bar"]<br>&nbsp;item = list[-1]<code> | `item == "bar"` |
| <center>==</center> | <code>list = ["foo"]<br>&nbsp;result = list == list<code> | `result == true` |
| <center>!=</center> | <code>result = ["foo"] != ["foo"]<code> | `result == true` |
| <center><</center> | <code>result = ["foo"] < ["foo"]<code> | `result == false` |
| <center>></center> | <code>result = ["foo"] > ["foo"]<code> | `result == false` |
| <center><=</center> | <code>result = ["foo"] <= ["foo"]<code> | `result == true` |
| <center>>=</center> | <code>result = ["foo"] >= ["foo"]<code> | `result == true` |
| <center>and</center> | <code>result = ["foo"] and ["foo"]<code> | `result == true` |
| <center>or</center> | <code>result = [] or ["foo"]<code> | `result == true` |

## Map

| Operator | Code | Result |
|---|---|---|
| <center>+</center> | <code>map = {"foo": 1, "bar": 2}<br>&nbsp;map = map + {"moo": 5}<code> | `map == {"foo": 1, "bar": 2, "moo": 5}` |
| <center>[x]</center> | <code>map = {"foo": 1, "bar": 2}<br>&nbsp;item = map.foo<code> | `item == 1` |
| <center>==</center> | <code>map = {"foo": 1, "bar": 2}<br>&nbsp;result = map == map<code> | `result == true` |
| <center>!=</center> | <code>result = {"foo": 1, "bar": 2} != {"foo": 1, "bar": 2}<code> | `result == true` |
| <center><</center> | <code>result = {"foo": 1, "bar": 2} < {"foo": 1, "bar": 2}<code> | `result == false` |
| <center>></center> | <code>result = {"foo": 1, "bar": 2} > {"foo": 1, "bar": 2}<code> | `result == false` |
| <center><=</center> | <code>result = {"foo": 1, "bar": 2} <= {"foo": 1, "bar": 2}<code> | `result == true` |
| <center>>=</center> | <code>result = {"foo": 1, "bar": 2} >= {"foo": 1, "bar": 2}<code> | `result == true` |
| <center>and</center> | <code>result = {"foo": 1, "bar": 2} and {"foo": 1, "bar": 2}<code> | `result == true` |
| <center>or</center> | <code>result = {} or {"foo": 1, "bar": 2}<code> | `result == true` |