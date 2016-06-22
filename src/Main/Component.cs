namespace Ge
{
    public abstract class Component
    {
        private GameObject _attachedGO;

        public GameObject GameObject => _attachedGO;

        public Transform Transform => _attachedGO.Transform;

        internal void AttachToGameObject(GameObject go, SystemRegistry registry)
        {
            _attachedGO = go;
            Attached(registry);
        }

        public abstract void Attached(SystemRegistry registry);
        public abstract void Removed(SystemRegistry registry);
    }
}