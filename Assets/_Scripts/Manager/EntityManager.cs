namespace Manager {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Helpers;
    using Structure;
    using Unit;
    using Utility;

    /// <summary>
    /// This static class provides a way for entities to register themselves using their collider as a key.
    /// This enables an entity lookup in physics related cases, e.g. from OverlapSphere.
    /// </summary>
    public static class EntityManager {

        private static readonly Dictionary<Collider, IEntity> _entities = new Dictionary<Collider, IEntity>(100);

        private static readonly Dictionary<Type, EntityType> _typesLookup = new Dictionary<Type, EntityType>() {
            { typeof(IUnit), EntityType.UNIT },
            { typeof(IStructure), EntityType.STRUCTURE }
        };

        public static void Link(this IEntity entity, Collider collider) {
            _entities[collider] = entity;
        }

        public static bool Unlink(this IEntity entity, Collider collider) {
            return _entities.Remove(collider);
        }

        public static IEnumerator<T> GetAllEntities<T>() where T : IEntity {
            var type = _typesLookup.GetValueOrDefault(typeof(T), EntityType.NONE);
            using(var enumerator = _entities.GetEnumerator()) {
                while(enumerator.MoveNext()) {
                    var ent = enumerator.Current.Value;
                    if(ent.entityType == type)
                        yield return (T)ent;
                }
            }
        }

        public static IEntity GetEntity(this Collider collider) {
            return _entities.GetValueOrDefault(collider);
        }

        public static T GetEntity<T>(this Collider collider) where T : class , IEntity {
            return GetEntity(collider) as T;
        }
    }
}