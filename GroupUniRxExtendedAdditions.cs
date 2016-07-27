using System;
using System.Collections.Generic;
using UniRx;

namespace Entitas {

    public class AnyEntityChangeEventArgs<T> : EventArgs {
        public GroupChangedEventArgs<T> addition;
        public GroupUpdatedEventArgs<T> update;
        public GroupChangedEventArgs<T> removal;
    }

    public partial class Group {

        /** A component of type T had any of the changes specified in 'changeTypes' within the group. The appropriate 'update', 'addition' or 'removal' field will be set in the resulting AnyEntityChangeEventArgs. */

        public IObservable<AnyEntityChangeEventArgs<T>> OnAnyEntityChangeObservable<T>(ChangeType changeTypes = ChangeType.Addition | ChangeType.Replacement | ChangeType.Removal){

            List<IObservable<AnyEntityChangeEventArgs<T>>> observedChanges = new List<IObservable<AnyEntityChangeEventArgs<T>>>();

            if((changeTypes & ChangeType.Addition) == ChangeType.Addition){
                var entityAdded = OnEntityAddedAsObservable<T>().Select<GroupChangedEventArgs<T>, AnyEntityChangeEventArgs<T>>(addition => {
                    var change = new AnyEntityChangeEventArgs<T>();
                    change.addition = addition;
                    return change;
                });
                observedChanges.Add(entityAdded);
            }

            if((changeTypes & ChangeType.Replacement) == ChangeType.Replacement){
                var entityUpdated = OnEntityUpdatedAsObservable<T>().Select<GroupUpdatedEventArgs<T>, AnyEntityChangeEventArgs<T>>(update => {
                    var change = new AnyEntityChangeEventArgs<T>();
                    change.update = update;
                    return change;
                });
                observedChanges.Add(entityUpdated);
            }       

            if((changeTypes & ChangeType.Removal) == ChangeType.Removal){
                var entityRemoved  = OnEntityRemovedAsObservable<T>().Select<GroupChangedEventArgs<T>, AnyEntityChangeEventArgs<T>>(removal => {
                    var change = new AnyEntityChangeEventArgs<T>();
                    change.removal = removal;
                    return change;
                });
                observedChanges.Add(entityRemoved);
            }

            return Observable.Merge(observedChanges);
        }
    }
}