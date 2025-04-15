namespace PartialArrays.Arrays
{
    public interface IPartialArray<T>
    {
        int Count { get; } //returns count objects in aaray
        int ControlledBytesLength { get; } //returns count of controlled bytes

        T this[int index] { get; set; }

        T GetElement(int id); //reeturns element from him id
        void SetElement(int id, T value); //set element with id
    }
}
