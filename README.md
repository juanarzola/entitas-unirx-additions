# Entitas UniRx Additions

[Entitas](https://github.com/neuecc/UniRx) is an excellent ECS framework that works with Unity. [UniRx](https://github.com/neuecc/UniRx) is a reactive programming framework for Unity. These are simple extensions that I wrote in order to be able to use UniRx's reactive programming with Entitas.  

They simply wrap Entita's built in C# events for changes in entities and groups. They use generics to limit changes to a component type.

With the Entity observables, you can react to component changes in an entity instance. They include:

 - `OnComponentAddedAsObservable<Component>` - A Component was added on the entity instance
 - `OnComponentReplacedAsObservable<Component>` - A Component value changed in the entity (it was replaced by another component of the same type)
 - `OnComponentRemovedAsObservable<Component>` = A Component was removed.

Group observables are similar to the Entity observables: you can react to component changes, but you do so on groups of entities that match a query.  This has the advantage that you don't need the entity instances for the reactive code, so you can do the setup up-front on System initialization before entity instances are available. They include:

 - `OnEntityAddedAsObservable<Component>` - On an entity within the group, a component of type Component was added
 - `OnEntityUpdatedAsObservable<Component>` - On an entity within the group, a component of type Component was replaced by a component of the same type.
 - `OnEntityRemovedAsObservable<Component>` - On an entity within the group, a component of type Component was removed.

## Extended additions

**Entity's `OnAnyChangeObservable\<Component\>` and Group's OnAnyEntityChangeObservable\<Component\>**

The `OnAny` extensions create observables for combinations of additions, removals and replacements. They generate `AnyEntityChangeEventArgs` objects with the data of the addition, removal or replacement - whichever happened.

**Typical usage:**

The Entity extensions can be used anywhere you have an Entity instance (Like the execute method of a reactive system):
 
```csharp
public class StepperSystem : IReactiveSystem, IEnsureComponents {
    
    public TriggerOnEvent trigger { get { return matcher().OnEntityAdded();  }}
    public IMatcher matcher(){return Matcher.AllOf(Matcher.SteppedMover);}
    public IMatcher ensureComponents {  get {  return Matcher.View; } }

    public void Execute(List<Entity> entities) {	
        foreach (var e in entities) {
            StartStepperTimer(e);
        }
    }
    private void StartStepperTimer(Entity e){

        // Add a timer after the stepper component is added. Timer is removed whenever the component is removed or replaced by another one.

        Observable.Timer(new DateTimeOffset(), new TimeSpan(0,0,1))
            .TakeUntil(e.OnAnyChangeObservable<Stepper>())
            .Subscribe(timerCount => {
                var position = e.position;
                var deltaAmount = 1.0f;
                Vector3 delta = Vector3.zero;
                switch(e.stepper.direction){
                    case stepper.Direction.Left:
                    delta = new Vector3(deltaAmount, 0, 0);
                    break;
                    case SteppedMover.Direction.Right:
                    delta = new Vector3(-deltaAmount, 0, 0);
                    break;
                    case SteppedMover.Direction.Up:
                    delta = new Vector3(0, deltaAmount, 0);
                    break;
                    case SteppedMover.Direction.Down:
                    delta = new Vector3(0, -deltaAmount, 0);
                    break;
                }
                e.ReplacePosition(position.x + delta.x, 
                                    position.y + delta.y, 
                                    position.z + delta.z);                                    
            }).AddTo(e.view.gameObject);
    }
}
```

The Group extensions are useful as the setup code of Initialize systems (Within SetPool):

```csharp
public class CleanupOrphanedEntitiesSystem : IInitializeSystem, ISetPool {
    
    public void SetPool(Pool pool){
        pool.GetGroup(Matcher.View)
            .OnEntityAddedAsObservable<View>()
            .Where(evt => evt.component.entityIsDestroyedWithGameObject)
            .Subscribe(evt => {	
                evt.entity.view.gameObject.OnDestroyAsObservable().Subscribe(_ => {
                    pool.DestroyEntity(evt.entity);
                });
            });
    }
    
    public void Initialize(){}
}
```
