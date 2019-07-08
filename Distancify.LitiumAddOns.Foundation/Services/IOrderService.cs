using System;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Runtime.DependencyInjection;
using System.Collections.Generic;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Deliveries;

namespace Distancify.LitiumAddOns.Services
{
    [Service(
    ServiceType = typeof(IOrderService),
    Lifetime = DependencyLifetime.Singleton)]
    public interface IOrderService
    {
        List<Order> GetOrders(short stateId);
        void UpdateOrderState(Guid orderId, short newStateId);
        void UpdateOrderState(Order order, short newStateId);
        void UpdateDeliveriesState(Guid orderId, short newStateId);
        void UpdateDeliveriesState(Order order, short newStateId);
        Order GetOrder(Guid id);
        void SetAdditionalOrderInfoValue(Order order, string key, string value);
        void SetAdditionalOrderInfoValue(OrderCarrier orderCarrier, string key, string value);
        bool CompletePayments(Order order);
        bool ReduceStockBalance(Order order, Guid inventorySystemId);
        bool ReduceStockBalance(Order order, List<Guid> inventorySystemIds);
        bool ReturnOrCancelAllPayments(Order order);
        bool CancelAllPayments(OrderCarrier orderCarrier);
        bool CancelAllPayments(Order order);
        DeliveryMethodCost GetDeliveryMethodCost(Guid deliveryMethodSystemId, Guid currencySystemId);
        Order GetOrderIfLoggedInUserIsLinkedToOrganizationOrThrowOrderAccessException(string orderGuid);
    }
}