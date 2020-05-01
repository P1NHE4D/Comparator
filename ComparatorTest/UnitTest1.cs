using System;
using System.Collections.Generic;
using Xunit;

namespace ComparatorTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Empty(new List<string>());
        }
        
        [Fact]
        public void Test2()
        {
            // Assert.Empty(new int []{1, 2});
        }
    }
}