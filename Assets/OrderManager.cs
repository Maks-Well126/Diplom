using System.Collections.Generic;
using UnityEngine;
public class OrderManager : MonoBehaviour
{
    [System.Serializable]
    public class Order
    {
        public string orderName;
        public Transform pickupPoint;  // Где забрать заказ
        public Transform deliveryPoint; // Куда доставить заказ
    }

    public List<Order> orders = new List<Order>(); // Список всех заказов
    private Order currentOrder; // Активный заказ

    public void SelectOrder(int index)
    {
        if (index < 0 || index >= orders.Count) return;

        currentOrder = orders[index];
        Debug.Log($"Заказ {currentOrder.orderName} взят! Идите в точку: {currentOrder.pickupPoint.position}");

        // Здесь можно добавить логику построения маршрута
    }

    public Order GetCurrentOrder()
    {
        return currentOrder;
    }
}

