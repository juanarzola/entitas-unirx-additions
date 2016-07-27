using System;
using System.Collections.Generic;
using UniRx;

namespace Entitas {
    
    public class AnyComponentChangeEventArgs<T> : EventArgs {
        public EntityChangedEventArgs<T> addition;
        public ComponentReplacedEventArgs<T> replacement;
        public EntityChangedEventArgs<T> removal;
    }

    public partial class Entity {

        /** A component of type T had any of the changes specified in 'changeTypes'. The appropriate 'replacement', 'addition' or 'removal' field will be set in the resulting AnyComponentChangeEventArgs. */

        public IObservable<AnyComponentChangeEventArgs<T>> OnAnyChangeObservable<T>(ChangeType changeTypes = ChangeType.Addition | ChangeType.Replacement | ChangeType.Removal){

            List<IObservable<AnyComponentChangeEventArgs<T>>> observedChanges = new List<IObservable<AnyComponentChangeEventArgs<T>>>();

            if((changeTypes & ChangeType.Addition) == ChangeType.Addition){
                var componentAdded  = OnComponentAddedAsObservable<T>().Select<EntityChangedEventArgs<T>, AnyComponentChangeEventArgs<T>>(addition => {
                    var change = new AnyComponentChangeEventArgs<T>();
                    change.addition = addition;
                    return change;
                });            
                observedChanges.Add(componentAdded);
            }

            if((changeTypes & ChangeType.Replacement) == ChangeType.Replacement){
                var componentReplaced = OnComponentReplacedAsObservable<T>().Select<ComponentReplacedEventArgs<T>, AnyComponentChangeEventArgs<T>>(replacement => {
                    var change = new AnyComponentChangeEventArgs<T>();
                    change.replacement = replacement;
                    return change;
                });
                observedChanges.Add(componentReplaced);
            }    

            if((changeTypes & ChangeType.Removal) == ChangeType.Removal){
                var componentRemoved  = OnComponentRemovedAsObservable<T>().Select<EntityChangedEventArgs<T>, AnyComponentChangeEventArgs<T>>(removal => {
                    var change = new AnyComponentChangeEventArgs<T>();
                    change.removal = removal;
                    return change;
                });            
                observedChanges.Add(componentRemoved);
            }

            return Observable.Merge(observedChanges);
        }
    }
}