using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager; // Asocia el AudioManager aquí

    // ...

    // Llama a este método para reproducir el sonido de pasos del enemigo
    public void PlayFootstepSound()
    {
        audioManager.Play("EnemyFootstepSound");
    }
}

