using EasyShop.Domain.InventoryItem;
using ErrorOr;
using FluentAssertions;

namespace EasyShop.Domain.UnitTests.InventoryItem;

public class InventoryItemTests
{
    [Theory]
    [InlineData(5, 6)]
    [InlineData(1, 2)]
    public void Reserve_WhenMoreThanAvailable_ShouldFail(int onhand, int reserve)
    {
        var inventory = new Domain.InventoryItem.InventoryItem(Guid.NewGuid(), Guid.NewGuid(), onhand);

        var result = inventory.Reserve(reserve);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(InventoryItemErrors.InvalidReserveQty);
    }
    
    [Theory]
    [InlineData(5, 3)]
    [InlineData(1, 1)]
    public void Reserve_WhenMoreLessThanOrEqualAvailable_ShouldSuccess(int onhand, int reserve)
    {
        var inventory = new Domain.InventoryItem.InventoryItem(Guid.NewGuid(), Guid.NewGuid(), onhand);

        var result = inventory.Reserve(reserve);

        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }
    
    [Theory]
    [InlineData(5, 5, 6)]
    [InlineData(1, 0, 6)]
    public void Release_WhenMoreThanReserved_ShouldFail(int onHand, int reserve, int release)
    {
        var inventory = new Domain.InventoryItem.InventoryItem(Guid.NewGuid(), Guid.NewGuid(), onHand);

        inventory.Reserve(reserve);

        var result = inventory.Release(release);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(InventoryItemErrors.InvalidReleaseQty);
    }
    
    
    [Theory]
    [InlineData(5, 4, 3)]
    [InlineData(5, 4, 2)]
    public void Release_WhenLessThanOrEqualToReserved_ShouldSuccess(int onHand, int reserve, int release)
    {
        var inventory = new Domain.InventoryItem.InventoryItem(Guid.NewGuid(), Guid.NewGuid(), onHand);

        inventory.Reserve(reserve);

        var result = inventory.Release(release);

        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }
    
}