using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Clawfoot.Status
{
    /*[AsyncMethodBuilder(typeof(MyTaskMethodBuilder<>))]
    public class StatusTask<T>
    {
        public Awaiter<T> GetAwaiter();
    }

    public class Awaiter<T> : INotifyCompletion
    {
        public bool IsCompleted { get; }
        public T GetResult();
        public void OnCompleted(Action completion);
    }

    public class MyTaskMethodBuilder<T>
    {
        public static MyTaskMethodBuilder<T> Create();

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine;

        public void SetStateMachine(IAsyncStateMachine stateMachine);
        public void SetException(Exception exception);
        public void SetResult(T result);

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;

        public StatusTask<T> Task { get; }
    }*/
}
