using Distancify.LitiumAddOns.Tasks.Synchronization;
using Litium;
using Litium.Customers;
using Litium.Foundation;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Foundation.Modules.ECommerce.Deliveries;
using Litium.Foundation.Modules.ECommerce.Orders;
using Litium.Foundation.Modules.ECommerce.Payments;
using Litium.Foundation.Modules.ECommerce.Search;
using Litium.Foundation.Search;
using Litium.Foundation.Security;
using Litium.Framework.Search;
using Litium.Globalization;
using Litium.Products;
using Litium.Security;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Distancify.LitiumAddOns.Foundation.Extensions;

namespace Distancify.LitiumAddOns.Services
{
    public class OrderService : IOrderService
    {
        public const string TaskSyncronizerGroupName = "Orders";

        private readonly ITaskSynchronizer _taskSynchronizer;        
        private readonly ModuleECommerce _moduleECommerce;
        private readonly VariantService _variantService;
        private readonly LanguageService _languageService;
        private readonly SecurityToken _token;
        private readonly SecurityContextService _securityContextService;
        private readonly PersonService _personService;
        private readonly InventoryItemService _inventoryItemService;

        public OrderService(ITaskSynchronizer taskSynchronizer, VariantService variantService, LanguageService languageService,
            ModuleECommerce moduleECommerce, SecurityContextService securityContextService, PersonService personService,
            InventoryItemService inventoryItemService)
        {
            _taskSynchronizer = taskSynchronizer;            
            _moduleECommerce = moduleECommerce;
            _variantService = variantService;
            _languageService = languageService;
            _token = Solution.Instance.SystemToken;
            _securityContextService = securityContextService;
            _personService = personService;
            _inventoryItemService = inventoryItemService;
    }

        public List<Order> GetOrders(short stateId)
        {
            var filterTags = new List<ITag>() {
                new MandatoryTagClause(new List<ITag>() {
                    new Tag(TagNames.OrderStatus, stateId)
                })
            };
            return GetOrders(filterTags);
        }

        private List<Order> GetOrders(List<ITag> filterTags)
        {
            var request = new QueryRequest(_languageService.Get(CultureInfo.CurrentCulture).SystemId, ECommerceSearchDomains.Orders, _token);
            foreach (var filterTag in filterTags) request.FilterTags.Add(filterTag);
            var searchService = IoC.Resolve<SearchService>();
            var searchResponse = searchService.Search(request);

            if (searchResponse.Hits.Any())
            {
                var orderIds = searchResponse.Hits.Select(h => Guid.Parse(h.Id));
                return _moduleECommerce.Orders.GetOrders(orderIds, _token).ToList();
            }
            return new List<Order>();
        }

        public void UpdateOrderState(Guid orderId, short newStateId)
        {
            var order = _moduleECommerce.Orders[orderId, _token];
            UpdateOrderState(order, newStateId);
        }

        public void UpdateOrderState(Order order, short newStateId)
        {
            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                order.SetOrderStatus(newStateId, _token);
            });
        }
        
        public void UpdateDeliveriesState(Guid orderId, short newStateId)
        {
            var order = _moduleECommerce.Orders[orderId, _token];
            UpdateDeliveriesState(order, newStateId);
        }
        
        public void UpdateDeliveriesState(Order order, short newStateId)
        {
            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                foreach (var delivery in order.Deliveries)
                {
                    delivery.SetDeliveryStatus(newStateId, _token);
                }
            });
        }

        public Order GetOrder(Guid id)
        {
            return _moduleECommerce.Orders[id, _token];
        }

        public void SetAdditionalOrderInfoValue(Order order, string key, string value)
        {
            var orderCarrier = order.GetAsCarrier(true, false, false, false, false, false);
            orderCarrier.SetAdditionalOrderInfoValue(key, value);
            order.SetValuesFromCarrier(orderCarrier, _token);
        }

        public void SetAdditionalOrderInfoValue(OrderCarrier orderCarrier, string key, string value)
        {
            var order = GetOrder(orderCarrier.ID);
            orderCarrier.SetAdditionalOrderInfoValue(key, value);
            order.SetValuesFromCarrier(orderCarrier, _token);
        }

        public bool CompletePayments(Order order)
        {
            var successful = false;

            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                try
                {
                    foreach (var paymentInfo in order.PaymentInfo)
                    {
                        if (paymentInfo.PaymentStatus != PaymentStatus.Paid && paymentInfo.PaymentProvider.CanCompleteCurrentTransaction)
                        {
                                paymentInfo.PaymentProvider.CompletePayment(null, _token);
                        }
                    }

                    successful = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Payment completion failed for order {OrderID}.", order.ExternalOrderID);
                }
            });

            return successful;
        }

        public bool ReduceStockBalance(Order order, Guid inventorySystemId)
        {
            return ReduceStockBalance(order, new List<Guid> { inventorySystemId });
        }

        public bool ReduceStockBalance(Order order, List<Guid> inventorySystemIds)
        {
            var successful = true;

            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                try
                {   
                    var inventoriesToUpdate = new List<InventoryItem>();
                    inventorySystemIds = inventorySystemIds.Distinct().ToList();

                    foreach (var orderRow in order.OrderRows)
                    {
                        var variant = _variantService.Get(orderRow.ArticleNumber)?.MakeWritableClone();
                        if (variant == null)
                        {
                            throw new Exception($"Tried to reduce stock article {orderRow.ArticleNumber} in Litium order {order.ExternalOrderID} but the variant does not exists.");
                        }

                        var stockLeftToReduce = orderRow.Quantity;
                        var variantInventoryItems = _inventoryItemService.GetByVariant(variant.SystemId);

                        foreach (var inventorySystemId in inventorySystemIds)
                        {
                            var inventoryItem = variantInventoryItems
                                .FirstOrDefault(x => inventorySystemId.Equals(x.InventorySystemId) && x.InStockQuantity > 0)?
                                .MakeWritableClone();

                            if (inventoryItem != null)
                            {
                                var stockToReduce = Math.Min(inventoryItem.InStockQuantity, stockLeftToReduce);
                                inventoryItem.InStockQuantity -= stockToReduce;
                                stockLeftToReduce -= stockToReduce;
                            }

                            if (stockLeftToReduce == 0)
                            {
                                inventoriesToUpdate.Add(inventoryItem);
                                break;
                            }
                        }

                        if (stockLeftToReduce > 0)
                        {
                            successful = false;
                            break;
                        }
                    }

                    if (successful)
                    {
                        using (Solution.Instance.SystemToken.Use())
                        {
                            foreach (var inventoryItem in inventoriesToUpdate)
                            {
                                _inventoryItemService.Update(inventoryItem);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    successful = false;
                    Log.Error(ex, "Could not reduce the stock quantity for order {OrderID}.", order.ExternalOrderID);
                }
            });

            return successful;
        }

        public bool ReturnOrCancelAllPayments(Order order)
        {
            var successful = false;

            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                try
                {
                    foreach (var paymentInfo in order.PaymentInfo)
                    {
                        if (paymentInfo.PaymentStatus != PaymentStatus.Cancelled && paymentInfo.PaymentStatus != PaymentStatus.Returned)
                        {
                            if (paymentInfo.PaymentProvider.CanReturnPayment)
                            {
                                if (!paymentInfo.PaymentProvider.ReturnPayment(null, _token))
                                {
                                    throw new Exception("Failed to return payment.");
                                }
                            }
                            else if (paymentInfo.PaymentProvider.CanCancelCurrentTransaction)
                            {
                                if (!paymentInfo.PaymentProvider.CancelPayment(null, _token))
                                {
                                    throw new Exception("Failed to cancel payment.");
                                }
                            }
                            else throw new Exception("Payment was neither returnable nor cancelable.");
                        }
                    }

                    successful = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Return or cancelling of payments failed for order {OrderID}.", order.ExternalOrderID);
                }
            });

            return successful;
        }

        public bool CancelAllPayments(OrderCarrier orderCarrier)
        {
            var order = GetOrder(orderCarrier.ID);

            return CancelAllPayments(order);
        }

        public bool CancelAllPayments(Order order)
        {
            var allPaymentsCancelled = false;

            _taskSynchronizer.Synchronize(TaskSyncronizerGroupName, () =>
            {
                try
                {
                    foreach (var paymentInfo in order.PaymentInfo)
                    {
                        if (paymentInfo.PaymentStatus != PaymentStatus.Cancelled)
                        {
                            if (paymentInfo.PaymentProvider.CanCancelCurrentTransaction)
                            {
                                if (!paymentInfo.PaymentProvider.CancelPayment(null, _token))
                                {
                                    throw new Exception("Failed to cancel payment.");
                                }
                            }
                            else throw new Exception("Payment was not cancelable.");
                        }
                    }

                    allPaymentsCancelled = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Cancelling of payments failed for order {OrderID}.", order.ExternalOrderID);
                }
            });

            return allPaymentsCancelled;
        }

        public DeliveryMethodCost GetDeliveryMethodCost(Guid deliveryMethodSystemId, Guid currencySystemId)
        {
            return _moduleECommerce.DeliveryMethods.Get(deliveryMethodSystemId, _moduleECommerce.AdminToken)?
                .GetCost(currencySystemId);
        }

        public Order GetOrderIfLoggedInUserIsLinkedToOrganizationOrThrowOrderAccessException(string orderGuid)
        {
            var order = GetOrderOrThrowOrderAccessException(orderGuid);
            var loggedInUserSystemId = _securityContextService.GetIdentityUserSystemId();

            if (!loggedInUserSystemId.HasValue)
            {
                throw new OrderAccessException("Not logged in");
            }

            var userIsLinkedToOrganization = _personService.Get(loggedInUserSystemId.Value)
                .OrganizationLinks
                .Any(organizationLink => organizationLink.OrganizationSystemId.Equals(order.CustomerInfo.OrganizationID));

            if (!userIsLinkedToOrganization)
            {
                throw new OrderAccessException($"Order does not belong to person's organization ({order.CustomerInfo.CustomerNumber})");
            }

            return order;
        }

        private Order GetOrderOrThrowOrderAccessException(string orderGuid)
        {
            if (Guid.TryParse(orderGuid, out var orderID))
            {
                var order = GetOrder(orderID);

                if (order is Order)
                {
                    return order;

                }
                throw new OrderAccessException($"Order {orderGuid} does not exist");
            }
            throw new OrderAccessException($"Unparsable Guid: {orderGuid}");
        }

        public class OrderAccessException : Exception
        {
            public OrderAccessException(string message) : base(message) { }
        }
    }
}