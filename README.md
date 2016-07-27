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

*Entity's OnAnyChangeObservable<Component> and Group's OnAnyEntityChangeObservable*

Sometimes you may want to combine observing for additions, removals and replacements in one expression.  It can be tricky to combine the base events with Merge() and subscribe to it because additions, removals and replacements streams are of different types.  In order to combine them into one stream, they need to be of the same type. The _OnAny_ extensions do this - they convert them to AnyEntityChangeEventArgs types.
