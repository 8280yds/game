using UnityEngine;

namespace Freamwork
{
    public class GameStart : MonoBehaviour
    {
        void Start()
        {
            GameManager.instance.start();
        }
    }
}