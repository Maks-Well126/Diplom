using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
 [Header("Слайдер громкости")]
    public Scrollbar Scrollbar;

    [Header("Аудио источник")]
    public AudioSource musicSource;

    void Start()
    {
        // Установить стартовое значение громкости
        Scrollbar.value = musicSource.volume;

        // Подписка на событие изменения слайдера
        Scrollbar.onValueChanged.AddListener(SetVolume);
    }

    // Функция для изменения громкости
    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
    }

    void OnDestroy()
    {
        // Удалить подписку при уничтожении
        Scrollbar.onValueChanged.RemoveListener(SetVolume);
    }
}
