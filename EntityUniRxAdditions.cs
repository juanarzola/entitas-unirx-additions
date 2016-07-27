using System;
using UniRx;

namespace Entitas {

    public class ComponentReplacedEventArgs<T> : EventArgs {
        public Entity entity;
        public int index;
        public T previous;
        public T current;
    }

    public class EntityChangedEventArgs<T> : EventArgs {
        public Entity entity;
        public int index;
        public T component;
    }

    public partial class Entity {

        // Component replaced observable
        public IObservable<ComponentReplacedEventArgs<T>> OnComponentReplacedAsObservable<T>(){
            var componentReplaced = Observable.FromEvent<ComponentReplaced, ComponentReplacedEventArgs<T>>(handler => {
                ComponentReplaced componentReplacedHandler = (entity, index, previousComponent, newComponent) => {
                    if(previousComponent is T){
                        var args = new ComponentReplacedEventArgs<T>();
                        args.entity = entity;
                        args.index = index;
                        args.previous = (T)previousComponent;
                        args.current = (T)newComponent;
                        handler(args);
                    }
                };
                return componentReplacedHandler; 
            },
            componentReplacedHandler => OnComponentReplaced += componentReplacedHandler,
            componentReplacedHandler => OnComponentReplaced -= componentReplacedHandler);
            return componentReplaced;
        }

        // Component added observable
        public IObservable<EntityChangedEventArgs<T>> OnComponentAddedAsObservable<T>(){
            var componentAdded = Observable.FromEvent<EntityChanged, EntityChangedEventArgs<T>>(handler => {
                EntityChanged componentAddedHandler = (entity, index, component) => {
                    if(component is T){
                        var args = new EntityChangedEventArgs<T>();
                        args.entity = entity;
                        args.index = index;
                        args.component = (T)component;
                        handler(args);
                    }
                };
                return componentAddedHandler;
            },
            componentAddedHandler => OnComponentAdded += componentAddedHandler,
            componentAddedHandler => OnComponentAdded -= componentAddedHandler);
            return componentAdded;
        }

        // Component removed observable
        public IObservable<EntityChangedEventArgs<T>> OnComponentRemovedAsObservable<T>(){
            var componentRemoved = Observable.FromEvent<EntityChanged, EntityChangedEventArgs<T>>(handler => {
                EntityChanged componentRemovedHandler = (entity, index, component) => {
                    if(component is T){
                        var args = new EntityChangedEventArgs<T>();
                        args.entity = entity;
                        args.index = index;
                        args.component = (T)component;
                        handler(args);
                    }
                };
                return componentRemovedHandler; 
            },
            componentRemovedHandler => OnComponentRemoved += componentRemovedHandler,
            componentRemovedHandler => OnComponentRemoved -= componentRemovedHandler);
            return componentRemoved;
        }
    }
}