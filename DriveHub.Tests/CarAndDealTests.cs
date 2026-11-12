using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHub.Web.Domain.Entities;
using DriveHub.Web.Domain.Enums;

namespace DriveHub.Tests;

public class CarAndDealTests
{
    [Fact]
    public void Reserve_InStockCar_BecomesReserved()
    {
        var car = new Car { Status = CarStatus.InStock };
        car.Reserve();
        Assert.Equal(CarStatus.Reserved, car.Status);
    }

    [Fact]
    public void Reserve_SoldCar_ThrowsException()
    {
        var car = new Car { Status = CarStatus.Sold };
        Assert.Throws<InvalidOperationException>(() => car.Reserve());
    }

    [Fact]
    public void Release_ReservedCar_ReturnsToStock()
    {
        var car = new Car { Status = CarStatus.Reserved };
        car.Release();
        Assert.Equal(CarStatus.InStock, car.Status);
    }

    [Fact]
    public void CalculateTotal_With10PercentDiscount_ReturnsCorrectPrice()
    {
        var deal = new Deal { Discount = 10 };
        deal.CalculateTotal(1_000_000m);
        Assert.Equal(900_000m, deal.TotalPrice);
    }

    [Fact]
    public void CalculateTotal_WithoutDiscount_ReturnsFullPrice()
    {
        var deal = new Deal { Discount = 0 };
        deal.CalculateTotal(500_000m);
        Assert.Equal(500_000m, deal.TotalPrice);
    }
}