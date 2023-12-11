using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Referencia al AudioManager
    private AudioManager audioManager;

    private void Start()
    {
        // Obtén la instancia del AudioManager
        audioManager = AudioManager.instance;
    }

    // Llama a este método para reproducir el sonido de disparo del jugador
    public void PlayShootSound()
    {
        audioManager.Play("PlayerShootSound");
    }
}
