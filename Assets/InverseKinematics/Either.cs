using System;

public abstract class Either<L, R>
{
  // Subclass implementation calls the appropriate continuation.
  public abstract T Match<T>(Func<L, T> Left, Func<R, T> Right);

  // Convenience wrapper for when the caller doesn't want to return a value
  // from the match expression.
  public void Match(Action<L> Left, Action<R> Right)
  {
    Match(
        Left: x => { Left(x); return 0; },
        Right: x => { Right(x); return 0; }
    );
  }
}


public class Left<L, R> : Either<L, R>
{
  L Value { get; set; }

  public Left(L Value)
  {
    this.Value = Value;
  }

  public override T Match<T>(Func<L, T> Left, Func<R, T> Right)
  {
    return Left(Value);
  }
}

public class Right<L, R> : Either<L, R>
{
  R Value { get; set; }

  public Right(R Value)
  {
    this.Value = Value;
  }

  public override T Match<T>(Func<L, T> Left, Func<R, T> Right)
  {
    return Right(Value);
  }
}