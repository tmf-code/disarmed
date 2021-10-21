using System;

public abstract class Option<T>
{

  public static Option<T> of(T value) => value switch
  {
    null => new None<T>(),
    _ => new Some<T>(value)
  };

  public abstract bool IsSome();
  public abstract bool IsNone();
  public abstract T Unwrap();
  public abstract T UnwrapOrDefault(T value);
  public abstract TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none);
  public abstract void Match(Action<T> some, Action none);

  public abstract Option<TResult> Map<TResult>(Func<T, TResult> mapper);
  public abstract void End(Action<T> ender);
}

public class Some<T> : Option<T>
{
  private readonly T value;

  public Some(T value)
  {
    if (value == null) throw new System.NullReferenceException();

    this.value = value;
  }

  public override bool IsSome() => true;
  public override bool IsNone() => false;

  public override T Unwrap() => value;
  public override T UnwrapOrDefault(T value) => value;

  public override Option<TResult> Map<TResult>(Func<T, TResult> mapper) => Option<TResult>.of(mapper(value));
  public override void End(Action<T> ender) => ender(value);

  public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => some(Unwrap());
  public override void Match(Action<T> some, Action none) => some(Unwrap());
}

public class None<T> : Option<T>
{
  public override bool IsSome() => false;
  public override bool IsNone() => true;

  public override T Unwrap() => throw new System.NullReferenceException();
  public override T UnwrapOrDefault(T value) => value;

  public override Option<TResult> Map<TResult>(Func<T, TResult> mapper) => new None<TResult>();
  public override void End(Action<T> ender) { }

  public override TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => none();
  public override void Match(Action<T> some, Action none) => none();
}