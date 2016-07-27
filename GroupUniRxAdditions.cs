using System;
using UniRx;

namespace Entitas {
    
    public class GroupChangedEventArgs<T> : EventArgs {
        public Group group;
        public Entity entity;
        public int index;
        public T component;
    }

    public class GroupUpdatedEventArgs<T> : EventArgs {
        public Group group;
        public Entity entity;
        public int index;
        public T previous;
        public T current;
    }

    public partial class Group {

        // Entity added observable
        public IObservable<GroupChangedEventArgs<T>> OnEntityAddedAsObservable<T>(){
            var entityAdded = Observable.FromEvent<GroupChanged, GroupChangedEventArgs<T>>(handler => {
                GroupChanged entityAddedHandler = (group, entity, index, component) => {
                    if(component is T){
                        var args = new GroupChangedEventArgs<T>();
                        args.group = group;
                        args.entity = entity;
                        args.index = index;
                        args.component = (T)component;
                        handler(args);
                    }
                };
                return entityAddedHandler; 
            },
            entityAddedHandler => OnEntityAdded += entityAddedHandler,
            entityAddedHandler => OnEntityAdded -= entityAddedHandler);
            return entityAdded;
        }

        // Entity removed observable
        public IObservable<GroupChangedEventArgs<T>> OnEntityRemovedAsObservable<T>(){
            var entityRemoved = Observable.FromEvent<GroupChanged, GroupChangedEventArgs<T>>(handler => {
                GroupChanged entityRemovedHandler = (group, entity, index, component) => {
                    if(component is T){
                        var args = new GroupChangedEventArgs<T>();
                        args.group = group;
                        args.entity = entity;
                        args.index = index;
                        args.component = (T)component;
                        handler(args);
                    }
                };
                return entityRemovedHandler; 
            },
            entityRemovedHandler => OnEntityRemoved += entityRemovedHandler,
            entityRemovedHandler => OnEntityRemoved -= entityRemovedHandler);
            return entityRemoved;
        }

        // Entity updated observable
        public IObservable<GroupUpdatedEventArgs<T>> OnEntityUpdatedAsObservable<T>(){
            var entityUpdated = Observable.FromEvent<GroupUpdated, GroupUpdatedEventArgs<T>>(handler => {
                GroupUpdated entityRemovedHandler = (group, entity, index, previous, current) => {
                    if(previous is T){
                        var args = new GroupUpdatedEventArgs<T>();
                        args.group = group;
                        args.entity = entity;
                        args.index = index;
                        args.previous = (T)previous;
                        args.current = (T)current;
                        handler(args);
                    }
                };
                return entityRemovedHandler; 
            },
            entityUpdatedHandler => OnEntityUpdated += entityUpdatedHandler,
            entityUpdatedHandler => OnEntityUpdated -= entityUpdatedHandler);
            return entityUpdated;
        }
    }
}