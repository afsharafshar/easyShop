using EasyShop.Domain.OrderAggregate;
using EasyShop.Domain.OrderAggregate.Events;
using ErrorOr;
using FluentAssertions;

namespace EasyShop.Domain.UnitTests.Order;

public class OrderTests
{

    [Fact]
    public void Confirm_WhenHasItem_ShouldSucceed()
    {
        var order = new OrderAggregate.Order(Guid.NewGuid());
        
        order.AddItem(TestConstants.TestConstants.Product.Id, TestConstants.TestConstants.Product.Qty,  TestConstants.TestConstants.Product.Price);
        
        var result = order.Confirm();
        
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void Confirm_WhenNotHaveItem_ShouldFail()
    {
        var order = new OrderAggregate.Order(Guid.NewGuid());
        
        var result = order.Confirm();
        
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrderErrors.OrderEmpty);
    }
    
    [Fact]
    public void Confirm_WhenIsCanceled_ShouldFail()
    {
        var order = new OrderAggregate.Order(Guid.NewGuid());
        
        order.Cancel();
        
        var result = order.Confirm();
        
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(OrderErrors.OrderMustBeDraft);
    }

    [Fact]
    public void Cancel_ShouldRaiseEvent()
    {
        var order = new OrderAggregate.Order(Guid.NewGuid());
        
        order.Cancel();

        var domainEvents = order.PopDomainEvents();
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<OrderCanceledEvent>();
    }
    
}