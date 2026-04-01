namespace Chipmunk.ComponentContainers
{
    public static class ComponentContainerExtension
    {
        public static T GetContainerComponent<T>(this IContainerComponent component, bool isDerived = false) where T : IContainerComponent
        {
            return component.ComponentContainer.GetComponent<T>(isDerived);
        }
        public static T GetCompo<T>(this IContainerComponent component, bool isDerived = false) where T : IContainerComponent
        {
            return component.ComponentContainer.GetComponent<T>(isDerived);
        }
        public static T Get<T>(this IContainerComponent component, bool isDerived = false) where T : IContainerComponent
        {
            return component.ComponentContainer.GetComponent<T>(isDerived);
        }
        public static bool TryGet<T>(this IContainerComponent component, out T compo, bool isDerived = false) where T : IContainerComponent
        {
            return component.ComponentContainer.TryGetComponent(out compo,isDerived);
        }
        public static T GetSubclassCompo<T>(this IContainerComponent component)
        {
            return component.ComponentContainer.GetSubclassComponent<T>();
        }
        public static bool TryGetSubclassComponent<T>(this IContainerComponent component,out T compo)
        {
            return component.ComponentContainer.TryGetSubclassComponent(out compo);
        }
    }
}