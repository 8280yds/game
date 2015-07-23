using UnityEngine;
using UnityEngine.UI;

namespace Freamwork
{
    public class GameStart : MonoBehaviour
    {
        private RectTransform ufoTF
        {
            get
            {
                if (m_ufoTF == null)
                {
                    m_ufoTF = GameObject.Find("UFO").transform as RectTransform;
                }
                return m_ufoTF;
            }
        }
        private RectTransform m_ufoTF;

        private RectTransform progressBarTF
        {
            get
            {
                if (m_progressBarTF == null)
                {
                    m_progressBarTF = GameObject.Find("ProgressBar").transform as RectTransform;
                }
                return m_progressBarTF;
            }
        }
        private RectTransform m_progressBarTF;

        private Text progressText
        {
            get
            {
                if (m_progressText == null)
                {
                    m_progressText = GameObject.Find("ProgressText").GetComponent<Text>();
                }
                return m_progressText;
            }
        }
        private Text m_progressText;

        //===================================================================
        void Start()
        {
            GameManager.instance.start();
        }

        void Update()
        {
            if (ufoTF.anchoredPosition.y > 20f || ufoTF.anchoredPosition.y < -20f)
            {
                ufoMoveVector *= -1f;
            }
            ufoTF.anchoredPosition += ufoMoveVector;

            progressBarTF.sizeDelta = new Vector2(400 * _progress / 100, progressBarTF.sizeDelta.y);
            progressBarTF.anchoredPosition = new Vector2((progressBarTF.sizeDelta.x - 400) / 2, 
                progressBarTF.anchoredPosition.y);
            progressText.text = _progressStr + _progress + "%";
        }
        private Vector2 ufoMoveVector = new Vector2(0, 0.5f);

        /// <summary>
        /// 设置进度数据
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="progressStr"></param>
        public static void setProgressData(int progress, string progressStr)
        {
            _progress = progress;
            _progressStr = progressStr;
        }
        private static int _progress = 0;
        private static string _progressStr = "";
    }
}