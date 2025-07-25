using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Voxel
{
    [DefaultExecutionOrder(-90)]
    public class ThreadsManager : MonoBehaviour
    {
        public const int MAX_THREADS = 1;
        public static ThreadsManager Instance;
        private Thread[] threads;
        private Queue<Action>[] queue;
        private Queue<Action> mainQueue, forcedMainQueue;
        private List<QueuedAction> queuedActions;
        public int[] queueSize;
        public int mainQueueSize, forcedMainQueueSize;
        private int lastQueue;

        public bool noInstant;

        public static int CountMain
        {
            get
            {
                return Instance.mainQueue.Count + Instance.forcedMainQueue.Count + Instance.queuedActions.Count;
            }
        }

        int altSize;
        public static int CountAlt
        {
            get
            {
                return Instance.altSize;
            }
        }


        private void Awake()
        {
            Instance = this;
            mainQueue = new();
            forcedMainQueue = new();
            queuedActions = new();
            queueSize = new int[MAX_THREADS];
            threads = new Thread[MAX_THREADS];
            queue = new Queue<Action>[MAX_THREADS];
            for (int i = 0; i < MAX_THREADS; i++)
            {
                queue[i] = new();
                threads[i] = new Thread(new ParameterizedThreadStart(ThreadUpdate));
                threads[i].Start(i);
                if (Application.isEditor && Application.isPlaying)
                {
#if UNITY_EDITOR
                    EditorApplication.playModeStateChanged += (mode) =>
                    {
                        if (mode == PlayModeStateChange.ExitingPlayMode)
                        {
                            Abort();
                        }
                    };
#endif
                }
            }
        }

        private void OnDisable()
        {
            Abort();
        }

        private void Abort()
        {
            if(threads == null)
            {
                return;
            }
            foreach(var thread in threads)
            {
                thread?.Abort();
            }
        }

        private void Update()
        {
            mainQueueSize = mainQueue.Count;
            forcedMainQueueSize = forcedMainQueue.Count;

            const int mainInvokedLimit = 5;
            int mainInvoked = 0;

            while(mainQueue.Count > 0 && mainInvoked < mainInvokedLimit)
            {
                mainQueue.Dequeue()?.Invoke();
                mainInvoked++;
            }

            while (forcedMainQueue.Count > 0)
            {
                forcedMainQueue.Dequeue()?.Invoke();
            }

            for (int i = 0; i < MAX_THREADS; i++)
            {
                queueSize[i] = queue[i].Count;
            }

            if(queuedActions.Count > 0)
            {
                List<QueuedAction> actions = new(queuedActions);
                foreach (var act in actions)
                {
                    if (UnityEngine.Time.time > act.time)
                    {
                        act.action?.Invoke();
                        queuedActions.Remove(act);
                    }
                }
            }
        }

        private void ThreadUpdate(object pass)
        {
            int index = (int)pass;
            while (true)
            {
                if (queue[index].Count > 0)
                {
                    if (queue[index].TryDequeue(out var action))
                    {
                        action?.Invoke();
                        Instance.altSize = SumAlt();
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void QueueMain(Action action) => QueueMain(action, false);
        public static void QueueInstantMain(Action action) => QueueMain(action, true);
        private static void QueueMain(Action action, bool forced)
        {
            if(forced && !Instance.noInstant)
            {
                Instance.forcedMainQueue.Enqueue(action);
            }
            else
            {
                Instance.mainQueue.Enqueue(action);
            }
        }

        public static void QueueAction(Action action)
        {
            Instance.lastQueue = (Instance.lastQueue + 1) % MAX_THREADS;
            QueueWorker(Instance.lastQueue, action);
        }

        public static void QueueWorker(int thread, Action action)
        {
            int _thread = thread % MAX_THREADS;
            Instance.queue[_thread].Enqueue(action);
            Instance.altSize = SumAlt();
        }
        public static void Wait(float milliseconds)
        {
            Stopwatch watch = new();
            watch.Start();
            while(watch.ElapsedMilliseconds < milliseconds)
            {
                Thread.Sleep(10);
            }
            watch.Stop();
            watch = null;
        }
        public static void QueueFutureAction(Action action, float time)
        {
            QueueMain(() =>
            {
                Instance.queuedActions.Add(new QueuedAction()
                {
                    action = action,
                    time = UnityEngine.Time.time + time
                });
            });
        }

        public static int SumAlt()
        {
            int sum = 0;
            foreach (var queue in Instance.queue)
            {
                sum += queue.Count;
            }
            return sum;
        }

        private class QueuedAction
        {
            public Action action;
            public float time;
        }
    }
}