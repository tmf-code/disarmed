using System;

public class Loadable<T>
{
  private Option<T> value;
  private readonly Option<Func<T>> load;

  public Loadable(Func<T> load)
  {
    value = new None<T>();
    this.load = new Some<Func<T>>(load);
  }

  public bool IsLoaded() => value.IsSome();
  public bool IsUnloaded() => !IsLoaded();
  public T UnwrapOrLoad()
  {
    Load();
    return value.Unwrap();
  }

  public void Load()
  {
    if (value.IsSome()) return;
    value = new Some<T>(load.Unwrap()());
  }
}