using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UObject = UnityEngine.Object;

namespace JimmyHaglund {
    [Serializable]
    public class ObjectField : IEquatable<object> {
        [SerializeField] protected UObject _valueObject = null;
        protected virtual Type ValueType => typeof(object);
#if (UNITY_EDITOR)
        [CustomPropertyDrawer(typeof(ObjectField), true)]
        public class InterfaceContainerDrawer : PropertyDrawer {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                var targetObject = property.serializedObject.targetObject;
                var targetObjectClassType = targetObject.GetType();
                var field = targetObjectClassType.GetField(property.propertyPath, BindingFlags.Public | BindingFlags.Instance);
                if (field == null) {
                    field = targetObjectClassType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field == null || !Attribute.IsDefined(field, typeof(SerializeField))) {
                        return;
                    }
                }
                var value = field.GetValue(targetObject);
                var target = value as ObjectField;
                var oldValue = target._valueObject;
                target._valueObject = TypedObjectField(position, label.text, target._valueObject, target.ValueType);
                if (oldValue != target._valueObject) {
                    EditorUtility.SetDirty(targetObject);
                }
            }

            protected UObject TypedObjectField(Rect position, string label, UObject displayValue, Type type) {
                var uObject = displayValue as UObject;
                label += " (" + type.Name + ")";
                UObject setObject = EditorGUI.ObjectField(position, label, uObject, typeof(UObject), true) as UObject;
                if (setObject == displayValue) return displayValue;
                if (setObject == null) {
                    return null;
                }
                if (setObject.GetType() == typeof(GameObject)) {
                    var gameObject = setObject as GameObject;
                    var component = gameObject.GetComponent(type);
                    if (component != null) return component;
                }
                if (type.IsInstanceOfType(setObject)) {
                    return setObject as UObject;
                }
                return displayValue;
            }
        }
#endif
    }

    [Serializable]
    public class InterfaceField<T> : ObjectField where T : class {
        private T _value;
        protected override Type ValueType => typeof(T);

        public T Value {
            set {
                _value = value;
                _valueObject = value as UObject;
            }
            get {
                if (_value == null && _valueObject != null) {
                    _value = _valueObject as T;
                }
                return _value;
            }
        }
    }
}