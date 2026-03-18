using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

int[] array = Enumerable.Range(0, 1000).ToArray();
while (true)
{
    // in case when I want to sum just part of this array like 50 elements
    // create new array, copy 50 elements from first array to new array, and then pass new array to function. THIS IS TOO EXPENSIVE 
    //int[] copy = new int[50];
    //Array.Copy(array, 0, copy, 0, 50);
    //Sum(copy)

    //Sum(array);
    Sum(new Span<int>(array, 0, 50)); /*if you want just 50 elements from array*/
}
static int Sum(Span<int> span)
{
    int sum = 0;
    foreach (int value in span) sum += value;
    return sum;
}

//static int Sum(int[] array)
//{
//    int sum = 0;
//    foreach (int i in array) sum += i;
//    return sum;
//}


//another way of doing it is using offset and lenght, which will on Assembly side pay the costs
//static int Sum(int[] array, int offset, int lenght)
//{
//    int sum = 0;
//    foreach (int i = offset; i < offset + lenght; i++) 
//    { 
//        sum += array[i]; 
//    }
//    return sum;
//}

MySpan<char> span = new MySpan<char>("Hello, World!".ToCharArray());
while (span.Lenght > 0)
{
    Console.WriteLine(span[0]);
    span = span.Slice(1);
}


readonly ref struct MySpan<T>
{

    private readonly ref T _reference;
    private readonly int _length;
    public int Lenght => _length;

    public MySpan(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        _reference = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
    }


    public MySpan(ref T reference) 
    {
        _reference = ref reference; 
        _length = 1;
    }

    public MySpan(ref T reference, int length) //MemoryMarshall.CreateSpan
    {
        _reference = ref reference;
        _length = length;   
    }

    public ref T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_length)
            {
                throw new IndexOutOfRangeException();
            }
            return ref Unsafe.Add(ref _reference, index);
        }
    }

    public MySpan<T> Slice(int offset)
    {
        if ((uint) offset > (uint)_length)
        {
            throw new ArgumentOutOfRangeException();
        }
        return new MySpan<T>(ref Unsafe.Add(ref _reference, offset), _length - offset);
    }

}