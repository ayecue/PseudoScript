# While

```
while (true)
	break
end while

index = 0

function test()
	index += 1
	return index < 10
end function

while (test())
	print("test")
end while
```