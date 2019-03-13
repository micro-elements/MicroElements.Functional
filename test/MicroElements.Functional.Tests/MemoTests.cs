using System;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class MemoTests
    {
        [Fact]
        public void MemoTest1()
        {
            var saved = DateTime.Now;
            var date = saved;

            var f = Prelude.Memoize(() => date.ToString());

            var res1 = f();

            date = DateTime.Now.AddDays(1);

            var res2 = f();

            Assert.True(res1 == res2);
        }
    }
}
