using System;
using UnityEngine;

namespace JimmyHaglund.Development.EditorInterfaces {
    public class InterfaceFieldExample : MonoBehaviour, IExample {
        [SerializeField] public IFExample PublicField = new IFExample(); // Visible in inspector
        [SerializeField] private IFExample _privateDisplayedField = new IFExample(); // Visible
        private IFExample _privateField = new IFExample(); // Hidden
        [SerializeField] [HideInInspector] private IFExample _hiddenField = new IFExample(); // Hidden

        // It's convenient to make a property to reference the interface.
        private IExample InterfaceReference => PublicField.Value;
    }

    public interface IExample {}
    [Serializable] public class IFExample : InterfaceField<IExample> { }
}