using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Tests;

public class RequestStateTests
{
    [Fact]
    public void TakeInWork_NewRequest_BecomesInProgress()
    {
        var request = new Request();
        request.TakeInWork(5);
        Assert.Equal(RequestStatus.InProgress, request.Status);
        Assert.Equal(5, request.ManagerId);
    }

    [Fact]
    public void Approve_NewRequest_ThrowsException()
    {
        var request = new Request();
        Assert.Throws<InvalidOperationException>(() => request.Approve());
    }

    [Fact]
    public void Reject_WithoutComment_ThrowsException()
    {
        var request = new Request { Status = RequestStatus.InProgress };
        Assert.Throws<ArgumentException>(() => request.Reject(""));
    }

    [Fact]
    public void Cancel_RejectedRequest_ThrowsException()
    {
        var request = new Request { Status = RequestStatus.Rejected };
        Assert.Throws<InvalidOperationException>(() => request.Cancel());
    }
}