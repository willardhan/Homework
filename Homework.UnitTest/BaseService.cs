using System;
using Xunit.Abstractions;

namespace Homework.UnitTest
{
    public class BaseService: IDisposable
    {
        private readonly ITestOutputHelper _iTestOutputHelper;
        private DateTime startTime;
        public BaseService(ITestOutputHelper iTestOutputHelper)
        {
            startTime = DateTime.Now;
            _iTestOutputHelper = iTestOutputHelper;
            _iTestOutputHelper.WriteLine($"start at {startTime:yyyy-MM-dd HH:mm:ss}");
        }

        public virtual void Dispose()
        {
            var disposeTime = DateTime.Now;
            var timeSpan = disposeTime - startTime;
            _iTestOutputHelper.WriteLine($"end at {disposeTime:yyyy-MM-dd HH:mm:ss}");
            _iTestOutputHelper.WriteLine($"time span {timeSpan.TotalSeconds:F4}s");
        }
    }
}