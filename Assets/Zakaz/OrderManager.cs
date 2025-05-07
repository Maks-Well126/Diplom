using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderManager : MonoBehaviour
{
    [System.Serializable]
    public class Order
    {
        public string orderName;
        public Transform pickupPoint;  // Где забрать заказ
        public Transform deliveryPoint; // Куда доставить заказ
    }

    public enum OrderStatus
    {
        WaitingForOrder,
        PickupOrder,
        DeliverOrder,
        OrderCompleted,
        OrderFailed
    }

    [Header("Order Settings")]
    public List<Order> orders = new List<Order>(); // Список всех заказов
    private Order currentOrder; // Активный заказ
    private OrderStatus currentStatus = OrderStatus.WaitingForOrder;
    public Transform player; // Ссылка на персонажа
    public float interactionDistance = 0.1f; // Расстояние взаимодействия

    [Header("Timer Settings")]
    public float pickupTimeLimit = 120f; // 2 минуты на взятие заказа
    public float deliveryTimeLimit = 180f; // 3 минуты на доставку
    private float currentPickupTimer;
    private float currentDeliveryTimer;
    private int currentScore = 0;
    private int totalScore = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI statusText; // Текст статуса заказа
    public TextMeshProUGUI orderNameText; // Название заказа
    public TextMeshProUGUI timerText; // Текст таймера
    public TextMeshProUGUI scoreText; // Текст очков
    public TextMeshProUGUI totalScoreText; // Текст общего количества очков
    public TextMeshProUGUI resultMessageText; // Текст результата выполнения заказа

    [Header("Timer Settings")]
    public float minTimeBetweenOrders = 5f;
    public float maxTimeBetweenOrders = 20f;
    private float timeUntilNextOrder;

    private int pickupScore = 0; // Очки за взятие заказа

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Не назначен персонаж в OrderManager!");
            return;
        }

        // Проверка UI элементов
        if (resultMessageText == null)
        {
            Debug.LogError("Не назначен Result Message Text в OrderManager!");
        }
        else
        {
            resultMessageText.gameObject.SetActive(false);
        }
        if (statusText == null)
        {
            Debug.LogError("Не назначен Status Text в OrderManager!");
        }
        if (orderNameText == null)
        {
            Debug.LogError("Не назначен Order Name Text в OrderManager!");
        }
        if (timerText == null)
        {
            Debug.LogError("Не назначен Timer Text в OrderManager!");
        }
        if (scoreText == null)
        {
            Debug.LogError("Не назначен Score Text в OrderManager!");
        }
        if (totalScoreText == null)
        {
            Debug.LogError("Не назначен Total Score Text в OrderManager!");
        }

        UpdateUI();
        StartCoroutine(WaitForNewOrder());
    }

    private void Update()
    {
        if (currentOrder != null && player != null)
        {
            // Обновление таймеров
            if (currentStatus == OrderStatus.PickupOrder)
            {
                currentPickupTimer -= Time.deltaTime;
                if (currentPickupTimer <= 0)
                {
                    FailOrder("Время на взятие заказа истекло!");
                }
            }
            else if (currentStatus == OrderStatus.DeliverOrder)
            {
                currentDeliveryTimer -= Time.deltaTime;
                if (currentDeliveryTimer <= 0)
                {
                    FailOrder("Время на доставку заказа истекло!");
                }
            }

            // Проверка расстояния до точки сбора
            if (currentStatus == OrderStatus.PickupOrder)
            {
                float distanceToPickup = Vector3.Distance(player.position, currentOrder.pickupPoint.position);
                if (distanceToPickup < interactionDistance)
                {
                    PickupOrder();
                }
            }
            // Проверка расстояния до точки доставки
            else if (currentStatus == OrderStatus.DeliverOrder)
            {
                float distanceToDelivery = Vector3.Distance(player.position, currentOrder.deliveryPoint.position);
                if (distanceToDelivery < interactionDistance)
                {
                    DeliverOrder();
                }
            }

            UpdateTimerUI();
        }
    }

    private void PickupOrder()
    {
        // Сохраняем очки за взятие заказа, но не добавляем к общему счету
        pickupScore = Mathf.FloorToInt(currentPickupTimer * 0.5f);
        currentScore = pickupScore; // Только для отображения текущих очков
        
        currentStatus = OrderStatus.DeliverOrder;
        currentDeliveryTimer = deliveryTimeLimit;
        UpdateUI();
        Debug.Log($"Заказ {currentOrder.orderName} забран! Получено очков за взятие: {pickupScore}. Идите в точку доставки: {currentOrder.deliveryPoint.position}");
    }

    private void DeliverOrder()
    {
        currentStatus = OrderStatus.OrderCompleted;
        // Начисляем очки за доставку и добавляем очки за взятие к общему счету
        int deliveryScore = Mathf.FloorToInt(currentDeliveryTimer);
        currentScore = pickupScore + deliveryScore; // Обновляем текущие очки
        totalScore += currentScore; // Добавляем все очки к общему счету
        UpdateUI();
        if (resultMessageText != null)
        {
            resultMessageText.text = $"Заказ выполнен (+{currentScore} очков)";
            StartCoroutine(ShowMessageForSeconds(4f));
        }
        Debug.Log($"Заказ {currentOrder.orderName} доставлен! Получено очков за доставку: {deliveryScore}. Всего очков за заказ: {currentScore}");
        currentOrder = null;
        pickupScore = 0; // Сбрасываем очки за взятие
        StartCoroutine(WaitForNewOrder());
    }

    private void FailOrder(string reason)
    {
        currentStatus = OrderStatus.OrderFailed;
        currentScore = -30; // Штраф за провал заказа
        totalScore += currentScore;
        pickupScore = 0; // Сбрасываем очки за взятие
        UpdateUI();
        if (resultMessageText != null)
        {
            resultMessageText.text = "Заказ провален (штраф -30)";
            StartCoroutine(ShowMessageForSeconds(4f));
        }
        Debug.Log($"Заказ {currentOrder.orderName} провален! Причина: {reason}");
        currentOrder = null;
        StartCoroutine(WaitForNewOrder());
    }

    private IEnumerator WaitForNewOrder()
    {
        currentStatus = OrderStatus.WaitingForOrder;
        currentScore = 0; // Сбрасываем очки только при ожидании нового заказа
        pickupScore = 0; // Сбрасываем очки за взятие
        UpdateUI();
        
        timeUntilNextOrder = Random.Range(minTimeBetweenOrders, maxTimeBetweenOrders);
        yield return new WaitForSeconds(timeUntilNextOrder);

        if (orders.Count > 0)
        {
            int randomIndex = Random.Range(0, orders.Count);
            currentOrder = orders[randomIndex];
            currentStatus = OrderStatus.PickupOrder;
            currentPickupTimer = pickupTimeLimit;
            UpdateUI();
            Debug.Log($"Новый заказ {currentOrder.orderName}! Идите в точку: {currentOrder.pickupPoint.position}");
        }
    }

    private IEnumerator ShowMessageForSeconds(float seconds)
    {
        if (resultMessageText != null)
        {
            resultMessageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            resultMessageText.text = "";
            resultMessageText.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        if (statusText != null)
        {
            switch (currentStatus)
            {
                case OrderStatus.WaitingForOrder:
                    statusText.text = "Ожидание заказа";
                    break;
                case OrderStatus.PickupOrder:
                    statusText.text = "Забрать заказ";
                    break;
                case OrderStatus.DeliverOrder:
                    statusText.text = "Доставить заказ";
                    break;
                case OrderStatus.OrderCompleted:
                    statusText.text = "Заказ выполнен";
                    break;
                case OrderStatus.OrderFailed:
                    statusText.text = "Заказ провален";
                    break;
            }
        }

        if (orderNameText != null)
        {
            orderNameText.text = currentOrder != null ? currentOrder.orderName : "Нет активного заказа";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Очки за заказ: {currentScore}";
        }

        if (totalScoreText != null)
        {
            totalScoreText.text = $"Всего очков: {totalScore}";
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            if (currentStatus == OrderStatus.WaitingForOrder || 
                currentStatus == OrderStatus.OrderCompleted || 
                currentStatus == OrderStatus.OrderFailed)
            {
                timerText.text = ""; // Очищаем текст таймера
            }
            else
            {
                float currentTimer = currentStatus == OrderStatus.PickupOrder ? currentPickupTimer : currentDeliveryTimer;
                int minutes = Mathf.FloorToInt(currentTimer / 60);
                int seconds = Mathf.FloorToInt(currentTimer % 60);
                timerText.text = $"Осталось времени: {minutes:00}:{seconds:00}";
            }
        }
    }
}

