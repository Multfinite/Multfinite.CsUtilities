namespace Multfinite.Utilities
{
    public delegate void ObjectDelegate<T>(object sender, T obj);
    public delegate void ObjectDelegate<T, U>(T obj1, U obj2);
    public delegate void KeyObjectDelegate<T, U>(object sender, T key, U obj);
    public delegate void SetterDelegate<T>(T value);
    public delegate T ReturnTypeDelegate<T>();
    public delegate T ReturnTypeDelegateArgs1<T, A1>(A1 arg1);
    public delegate T ReturnTypeDelegateArgs2<T, A1, A2>(A1 arg1, A2 arg2);
    public delegate T ReturnTypeDelegateArgs3<T, A1, A2, A3>(A1 arg1, A2 arg2, A3 arg3);
    public delegate void ActionArgs1<A1>(A1 arg1);
    public delegate void ActionArgs2<A1, A2>(A1 arg1, A2 arg2);

	public delegate void CancelableAction<T>(T value, ref bool cancel);
}