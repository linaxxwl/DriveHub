using DriveHub.Web.Services;

namespace DriveHub.Tests;

public class TestDriveTests
{
    [Fact]
    public void Overlap_FiveMinutesApart_ReturnsTrue()
    {
        // 18:45 и 18:50: машина занята
        Assert.True(RequestService.TestDrivesOverlap(new TimeSpan(18, 45, 0), new TimeSpan(18, 50, 0)));
    }

    [Fact]
    public void Overlap_OneHourApart_ReturnsFalse()
    {
        // 18:00 и 19:00: первый закончился ровно к началу второго
        Assert.False(RequestService.TestDrivesOverlap(new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0)));
    }

    //для коммита
    [Fact]
    public void Overlap_FarApart_ReturnsFalse()
    {
        Assert.False(RequestService.TestDrivesOverlap(new TimeSpan(10, 0, 0), new TimeSpan(15, 30, 0)));
    }
}