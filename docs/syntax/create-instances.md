# Create instances

```
map = {}
map.classID = "test"
map.myFunc = function()
	return "test"
end function

instance = new map
print(instance.myFunc()) //returns "test"
```