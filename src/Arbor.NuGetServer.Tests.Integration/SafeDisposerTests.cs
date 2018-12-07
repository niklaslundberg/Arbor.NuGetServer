using System;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Xunit;

namespace Arbor.NuGetServer.Tests.Integration
{
    public class SafeDisposerTests
    {
        private sealed class ExceptionHandler
        {
            public bool IsHandled { get; private set; }

            public void Handle(Exception ex)
            {
                IsHandled = true;
            }
        }

        private sealed class TestDisposer : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        private sealed class TestThrowDisposer : IDisposable
        {
            public void Dispose()
            {
                throw new InvalidOperationException();
            }
        }

        [Fact]
        public void WhenDisposeThrowsException_ItShouldBeIgnoredWithoutHandler()
        {
            var testDisposer = new TestThrowDisposer();

            Exception exception = null;
            try
            {
                testDisposer.SafeDispose();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }

        [Fact]
        public void WhenDisposeThrowsExceptionAndAHandlerIsUsed_ItShouldBeHandled()
        {
            var testDisposer = new TestThrowDisposer();
            var handler = new ExceptionHandler();
            testDisposer.SafeDispose(handler.Handle);
            Assert.True(handler.IsHandled);
        }

        [Fact]
        public void WhenDisposeThrowsExceptionAndNullHandlerIsUsed_ItShouldNotThrow()
        {
            var testDisposer = new TestThrowDisposer();
            Exception exception = null;

            try
            {
                testDisposer.SafeDispose(null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }

        [Fact]
        public void WhenDisposingInstance_DisposedShouldHaveBeenCalled()
        {
            var testDisposer = new TestDisposer();

            testDisposer.SafeDispose();

            Assert.True(testDisposer.Disposed);
        }

        [Fact]
        public void WhenDisposingNull_ItShouldReturn()
        {
            ((IDisposable)null).SafeDispose();
        }
    }
}
