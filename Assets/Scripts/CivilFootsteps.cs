using UnityEngine;

public class CivilFootsteps : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager; // Asocia el AudioManager aquí

    // ...

    // Llama a este método para reproducir el sonido de pasos del civil
    public void PlayFootstepSound()
    {
        audioManager.Play("CivilFootstepSound");
    }
}
