using System;
using Tapas;

namespace MyTests
{
  public class Test_assert_equal {
    public Test_assert_equal() {

    }

    [Test]
    public void Equal() {
      Assert.Equal(1, 1);
    }

    [Test]
    public void Not_equal() {
      Assert.Equal(2, 1);
    }

    [Test]
    public void Multiple_valid_assert() {
      Assert.Equal(1, 1);
      Assert.Equal(2, 2);
    }

    [Test]
    public void Second_assert_should_not_be_called() {
      Assert.Equal(3, 1);
      Assert.Equal(4, 1);
    }
  }

  public class Test_assert_throw {
    public Test_assert_throw() {

    }

    [Test]
    public void Exception_with_a_message() {
      Exception ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException("exception 1"));
      Assert.Equal("exception 1", ex.Message);
    }

    [Test]
    public void Exception_with_the_original_message() {
      Exception ex = Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException());
      Assert.Equal("Operation is not valid due to the current state of the object.", ex.Message);
    }

    [Test]
    public void Throw_the_wrong_exception_type() {
      Exception ex = Assert.Throws<InvalidOperationException>(() => throw new ArgumentException());
    }

    [Test]
    public void Second_assert_should_not_be_called() {
      Exception ex = Assert.Throws<InvalidOperationException>(() => throw new ArgumentException());
      Assert.Equal("This should not be called", "");
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
