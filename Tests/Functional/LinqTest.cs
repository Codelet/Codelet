//namespace Codelet.Functional
//{
//  using System;
//  using System.Globalization;
//  using System.Threading.Tasks;
//  using Codelet.Testing;
//  using FluentAssertions;
//  using Moq;
//  using Ploeh.AutoFixture.Xunit2;
//  using Xunit;

//  public static class A
//  {
//    //public static async Task<TResult> Select<T, TResult>(
//    //  this Task<Maybe<T>> task,
//    //  Func<T, TResult> selector)
//    //{
//    //  task = task ?? throw new ArgumentNullException(nameof(task));
//    //  selector = selector ?? throw new ArgumentNullException(nameof(selector));

//    //  return null;
//    //}

//    public static Task<Maybe<TResult>> SelectMany<T, T1, TResult>(
//      this Task<T> task,
//      Func<T, Task<Maybe<T1>>> selector1,
//      Func<T, T1, TResult> selector)
//    {
//      return null;
//    }
//  }

//  public class LinqTest
//  {
//    public class Select
//    {
//      [Fact]
//      public async Task ReturnsNewValue(Task<int> task, Maybe<int> maybe)
//      {
//        var rrr = from a in (from a in Task.FromResult(maybe) select a)
//                  select a.ToString();

//        var rr =
//          from a in Task.FromResult(10)
//          from b in Task.FromResult(10.5)
//          let c = a.ToString()

//          let d = a.ToString()
//          select Task.FromResult("20.5");
//      }
//    }
//  }
//}