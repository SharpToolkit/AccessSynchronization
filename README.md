# Access Synchronization

A non invasive concurrency access synchronizer. It allows to easily manage objects in multithreaded environment by using safe functional approach with minimal impact on the code and an automatic deadlock discovery.

## Quick example

Add the locked version of an object to your model:

```CSharp
public class Model
{
    public int Value { get; set; }

    public Locked<Model> LockedModel { get; }

    public Model()
    {
        this.LockedModel = new Locked<Model>(this);
    }
}

```

Synchronize the access:

```CSharp
var model = new Model().LockedModel;

model.UnlockExclusive(x => x.Value = 42);

Console.WriteLine(model.Unlock(x => x.Value));
```


## Usage

### The locked object

A `Locked<T>` class is the entry point of the synchronization. It represents the object in a locked state. Unlike regular `lock` keyword semantics, you don't 'enter' or 'take' the lock. Here the meaning as in 'locked box'. While the box is locked you can't interact with it's contents and it needs to be unlocked first.

The creation of `Locked<T>` is trivial:
```CSharp
// Outside the object
var locked = new Locked<object>(new object);
// Or inside
this.Locked = new Locked<SomeType>(this);
```

You may now pass the locked object around. In order to then access the object you call one of the unlock methods:
```CSharp
locked.Unlock(x => { /* some code */ });
```

While inside the lambda the object is access is synchronized. There are three synchronization modes: *shared*, *upgradeable* and *exclusive*. Those are entered with `Unlock`, `UnlockUpgradeable` and `UnlockExclusive` respectevely. Under the hood, the synchronization is done by `ReaderWriterLockSlim` using the *read*, *upgradeable read* and *write* lock modes, so the same locking rules apply here.

**Important: For performance reasons, the lambda is not wrapped by a `try` block, and if the passed lambda will throw an exception, the program will enter an undefined state, as taken locks will not be released. It is your responsibility to handle the exceptions inside the lambda.**

---

There are four signatures for each unlock method:
```CSharp
TResult Unlock<TResult>(Func<TResult> fn);
TResult Unlock<TResult>(Func<T, TResult> fn);
void Unlock(Action action);
void Unlock(Action<T> action);
```

The func signatures are used to return values from the lambda. Each lambda can optionaly receive the unlocked object as as argument. The ones without the argument are meant to be used when you already have the reference to the unlocked object, mostly when using it inside a locked object method.

### Recursive locking

Trying to unlock an object more than once with the same or a less strict mode is transparent:
```CSharp
locked.Unlock(
	x => locked.Unlock(
		y => { /* Some code */ }));
```
The second unlock just invokes the lambda with minimal overhead.

---

Upgrading the mode is straightforward:
```CSharp
locked.UnlockUpgradeable(
	x => locked.UnlockExclusive(
		y => { /* Some code */ }));
```

---

Doing an illegal unlocking, like trying unlock object exclusevely while having the object unlocked in shared mode will throw `LockRecursionException`:

```CSharp
locked.Unlock(
	// Will throw
	x => locked.UnlockExclusive(
		y => { /* Some code */ }));
```
**Do not catch this exception, it is only meant to notify about wrong code while debugging. Catching it will cause an undefined state.**

### Relatives

In cases when you have a model which requires chaining the synchronized access, it can be easily done. Suppose you have a `Parent` and `Child` classes, and calls to `Child`'s methods require `Parent` to be synchronized:

```CSharp
var parent = new Locked<Parent>(new Parent());
var child = new Locked<Child>(new Child(), new [] { parent });
```

Note passing the parent to child's `Locked<T>` constructor. Each locked object passed to another locked object is called 'relative'. Relatives are unlocked before the target object:

```CSharp
child.Unlock(x => { /* Some code */ });
```
In this case, parent will be unlocked first. If child had more relatives, those would all be unlocked before the child. If parent had relatives, those would be unlocked before it, so the unlock chain would look like `Grandparent > Parent > Child`.

---

By default, relatives are unlocked in the same mode as the target object, but this can be overriden:
```CSharp
child.UnlockExclusiveWithRelativesAs(
	x => x.Unlock,
	x => { /* Some code */ });
```
Here, you choose what method to invoke when unlocking the relatives. In this example child's relatives would be unlocked using the `Unlock` method, while the child itself would be unlocked exclusevely.

Since the relatives are passed in a constructor and can't be changed during the course of `Locked<T>` lifetime, effectively a directed acyclic graph is created that forms a polytree, which prevents deadlocks from happening, unless you try to unlock one branch from an unlock of another branch. In that case resolvers will help you to detect such cases.

### Resolvers

Resolvers are defined by `ILockResolver` interface and used to choose a course of action when an object can't be unlocked. By default, in order to increase performance, the synchronization mechanism doesn't use any resolver. When a resolver is used, the system tracks all the unlocks that occur in current AppDomain, which creates a measurable overhead. When the unlocking exceeds the timeout defined by the resolver, the resolver is invoked, passing to it all the tracked information.

In order to use a resolver, just pass it into a `Locked<T>` constructor:
```CSharp
var locked = new Locked<object>(
	new object(), 
	Enumerable.Empty<ILockedObject>,
	resolver);
```

The library comes with an `ExceptionDeadlockResolver`. It throws a `DeadlockException` when it detects one, with the relevant data about the deadlock participants. Again, this exception should not be caught, but handled while debugging the program, and the code that lead to the deadlock should be fixed.

---

Currently if the resolver's `Resolve` method returns, the systems just tries to unlock the object again. In the future a backoff mechanism would be included.