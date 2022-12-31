using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.GodConsole
{
    public class InGameConsoleLoggerView : MonoBehaviour
    {
        [SerializeField] private InGameLogElement logElement;
        [SerializeField] private int maxLogCount = 20;

        public bool IsListening = false;

        private Queue<InGameLogElement> logQueue;

        private void Start()
        {
            logQueue = new(maxLogCount);
            logElement.gameObject.SetActive(false);
            for (int i = 0; i < maxLogCount; i++)
            {
                logQueue.Enqueue(Instantiate(logElement, logElement.transform.parent));
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Application_logMessageReceived;
        }

        private void Application_logMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (!IsListening) return;

            var element = logQueue.Dequeue();
            logQueue.Enqueue(element);

            element.Show(logString, type);
            element.transform.SetAsLastSibling();
        }
    }
}
