# Tapas C#

A minimalist yet complete C# testing library that output TAP format to be combined with other tools.
- Perfect for CI or TDD.
- Compatible with any .Net targets including Mono.

![Tapas](/Doc/bruschetta.png)

## Example

```cs
using System;
using Tapas;

namespace MyTests
{
  public class Test_assert_equal {
    [Test]
    public void Equal() {
      Assert.Equal(1, 1);
    }

    [Test]
    public void Not_equal() {
      Assert.Equal(2, 1);
    }
  }

  public class Test_assert_throw {
    [Test]
    public void Exception_with_a_message() {
      Exception ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("My message"));
      Assert.Equal("My message", ex.Message);
    }
  }

  class MainClass
  {
    public static void Main(string[] args)
    {
      TapasRunner.Create(args)
        .AddTest<Test_assert_equal>()
        .AddTest<Test_assert_throw>()
        .Run();
    }
  }
}
```

```
$ mono --debug Project/bin/Debug/Project.exe
TAP version 13
# MyTests.Test_assert_equal.Equal
ok 1 should be equal
# MyTests.Test_assert_equal.Not_equal
not ok 2 should be equal
  ---
  operator: equal
  expected: 2
  actual: 1
  at: /Users/user/Project/Project/Program.cs:13:7
  ...
# MyTests.Test_assert_throw.Exception_with_a_message
ok 3 should throw the expected exception
ok 4 should be equal

1..4
# tests 4
# pass 2
# fail 2
```

## Usage

You just need to create a console project, paste the example code, add the library by dropping [the library file](/Tapas/Tapas.cs) and directly run your executable. This makes it work on any platform.

Example with mono:
```
$ msbuild /p:Configuration=Debug /nologo /verbosity:quiet Project.csproj
$ mono --debug Project.exe
```

You can filter the tests by matching a sub-string of the titles:
```
$ mono --debug Project.exe --filter "MyTests.Test_assert_equal"
```

You can combine some commands to have a nice TDD environment. The following code compile on change, run the tests, send you a desktop notification and colorize the output in the terminal:
```
$ ag -l | entr -s ' \
  msbuild /p:Configuration=Debug /nologo /verbosity:quiet Project/Project.csproj && \
  mono --debug Project/bin/Debug/Project.exe | \
  tap-notify | \
  faucet \
'
```

Explanation:
- `ag -l` list all your project files based on your git repository ([ag](https://github.com/ggreer/the_silver_searcher))
- `entr` watches the files taken in input and execute a command ([entr](http://eradman.com/entrproject/))
- `msbuild` build the project
- `mono` runs
- `tap-notify` send the notification ([tap-notify](https://github.com/axross/tap-notify))
- `faucet` colorize the outpyt ([faucet](https://github.com/substack/faucet))

You can easily add this into a script or a Makefile.

## Pretty reporters

The default TAP output is good for machines and humans that are robots.

If you want a more colorful / pretty output there are lots of modules on npm
that will output something pretty if you pipe TAP into them:

- [tap-spec](https://github.com/scottcorgan/tap-spec)
- [tap-dot](https://github.com/scottcorgan/tap-dot)
- [faucet](https://github.com/substack/faucet)
- [tap-bail](https://github.com/juliangruber/tap-bail)
- [tap-browser-color](https://github.com/kirbysayshi/tap-browser-color)
- [tap-json](https://github.com/gummesson/tap-json)
- [tap-min](https://github.com/derhuerst/tap-min)
- [tap-nyan](https://github.com/calvinmetcalf/tap-nyan)
- [tap-pessimist](https://www.npmjs.org/package/tap-pessimist)
- [tap-prettify](https://github.com/toolness/tap-prettify)
- [colortape](https://github.com/shuhei/colortape)
- [tap-xunit](https://github.com/aghassemi/tap-xunit)
- [tap-difflet](https://github.com/namuol/tap-difflet)
- [tape-dom](https://github.com/gritzko/tape-dom)
- [tap-diff](https://github.com/axross/tap-diff)
- [tap-notify](https://github.com/axross/tap-notify)
- [tap-summary](https://github.com/zoubin/tap-summary)
- [tap-markdown](https://github.com/Hypercubed/tap-markdown)
- [tap-html](https://github.com/gabrielcsapo/tap-html)
- [tap-react-browser](https://github.com/mcnuttandrew/tap-react-browser)
- [tap-junit](https://github.com/dhershman1/tap-junit)

To use them, try `mono --debug Project.exe | faucet` or pipe it into one of the modules of your choice!

## Some reading

- [TDD Changed My Life](https://medium.com/javascript-scene/tdd-changed-my-life-5af0ce099f80)

## Credit

- Inspired by [Tape](https://github.com/substack/tape/blob/master/readme.markdown)
- <div>Icons made by <a href="https://www.freepik.com/" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/"                 title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/"                 title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a></div>
