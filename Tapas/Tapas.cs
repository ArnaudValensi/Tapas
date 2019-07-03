using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tapas
{
  public static class TapasRunner
  {
    public static int nbTests;
    public static int passedTests;

    public static Context Create(string[] args) {
      Regex regex = new Regex(@"(?:--|\/)filter=(.+)");

      string filterArg =
        args.ToList().FirstOrDefault(arg => regex.Match(arg).Success) ?? "";

      var match = regex.Match(filterArg);
      if (match.Success) {
        var filter = match.Groups[1];
        return new Context(filter.ToString());
      }

      return new Context();
    }
  }

  public class Context {
    List<Type> types;
    string filter;

    public Context(string filter = "") {
      types = new List<Type>();
      this.filter = filter;
    }

    public Context AddTest<T>() where T : new() {
      types.Add(typeof(T));
      return this;
    }

    public void Run() {
      // Version of the TAP spec: https://testanything.org/tap-specification.html
      Console.WriteLine("TAP version 13");

      foreach (var type in types) {
        RunOne(type);
      }

      // Plan of the TAP spec.
      Console.WriteLine("");
      Console.WriteLine(TapasRunner.nbTests > 0 ? $"1..{TapasRunner.nbTests}" : "0");

      // Summary.
      Console.WriteLine($"# tests {TapasRunner.nbTests}");
      Console.WriteLine($"# pass {TapasRunner.passedTests}");
      Console.WriteLine($"# fail {TapasRunner.nbTests - TapasRunner.passedTests}");
    }

    public void RunOne(Type type) {
      // Get all methods with the [Test] custom attric]bute.
      var methods = from method in type.GetMethods()
        where Attribute.IsDefined(method, typeof(Test))
        select method;

      // Get public constructors.
      var ctor = type.GetConstructor(Type.EmptyTypes);
      // Invoke the first public constructor with no parameters.
      var obj = ctor.Invoke(new object[] { });

      foreach (var method in methods) {
        // Construct the test name.
        var testName = $"# {method.DeclaringType.FullName}.{method.Name}";

        if (testName.Contains(filter)) {
          // Print the method name with full namespace and class.
          Console.WriteLine(testName);

          try {
            method.Invoke(obj, null);
          } catch (TargetInvocationException e) {
            // ExpetecException is used to abort the invoked test function at the first failed assert.
            if (e.InnerException.GetType() != typeof(ExpectedException)) {
              throw e.InnerException;
            }
          }
        }
      }
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class Test : Attribute { }

  public class ExpectedException : Exception { }

  public static class Assert {
    public static void WriteLocation() {
      var stackFrame = new StackTrace(2, true).GetFrame(0);
      var file = stackFrame.GetFileName();
      var line = stackFrame.GetFileLineNumber();
      var column = stackFrame.GetFileColumnNumber();

      if (!string.IsNullOrEmpty(file)) {
        Console.WriteLine($"  at: {file}:{line}:{column}");
      } else {
        Console.WriteLine($"  at: unavailable - compile and run in debug mode to have this");
      }
    }

    public static void Equal(object expected, object actual) {
      TapasRunner.nbTests++;

      if (expected.Equals(actual)) {
        Console.WriteLine($"ok {TapasRunner.nbTests} should be equal");
        TapasRunner.passedTests++;
      } else {
        Console.WriteLine($"not ok {TapasRunner.nbTests} should be equal");
        Console.WriteLine("  ---");
        Console.WriteLine($"  operator: equal");
        Console.WriteLine($"  expected: {expected.ToString()}");
        Console.WriteLine($"  actual: {actual.ToString()}");
        WriteLocation();
        Console.WriteLine("  ...");

        throw new ExpectedException();
      }
    }

    public static Exception Throws<T>(Action action) {
      TapasRunner.nbTests++;

      var expectedType = typeof(T);

      try {
        action.Invoke();
      } catch (Exception e) {
        var actualType = e.GetType();

        if (expectedType != actualType) {
          Console.WriteLine($"not ok {TapasRunner.nbTests} should throw the expected exception");
          Console.WriteLine("  ---");
          Console.WriteLine($"  operator: throw");
          Console.WriteLine($"  expected: {expectedType.ToString()}");
          Console.WriteLine($"  actual: {actualType.ToString()}");
          WriteLocation();
          Console.WriteLine("  ...");

          throw new ExpectedException();
        }

        Console.WriteLine($"ok {TapasRunner.nbTests} should throw the expected exception");
        return e;
      }

      Console.WriteLine($"not ok {TapasRunner.nbTests} should throw the expected exception");
      Console.WriteLine("  ---");
      Console.WriteLine($"  operator: throw");
      Console.WriteLine($"  expected: {expectedType.ToString()}");
      Console.WriteLine($"  actual: none");
      WriteLocation();
      Console.WriteLine("  ...");

      throw new ExpectedException();
    }
  }
}
