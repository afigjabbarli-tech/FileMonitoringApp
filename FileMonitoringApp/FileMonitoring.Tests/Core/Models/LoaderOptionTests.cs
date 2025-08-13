using FileMonitoring.Core.Models;

namespace FileMonitoring.Tests.Core.Models
{
    public class LoaderOptionTests
    {
        [Fact]
        public void SettingIsActive_ShouldRaisePropertyChanged()
        {
            var loaderOption = new LoaderOption { Name = "Test" };
            bool eventRaised = false;

            loaderOption.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LoaderOption.IsActive))
                    eventRaised = true;
            };

            loaderOption.IsActive = true;

            Assert.True(eventRaised);
        }
    }
}
