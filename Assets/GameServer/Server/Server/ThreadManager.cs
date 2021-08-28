using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ThreadManager
    {

        public static readonly List<Action> executeOnMainThread = new List<Action>();
        public static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        public static bool actionToExecuteOnMainThread = false;


        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Console.WriteLine("No action to execute on main thread! ");
                return;
            }
            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }
                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}
