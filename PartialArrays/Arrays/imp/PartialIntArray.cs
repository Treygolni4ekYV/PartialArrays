using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using PartialArrays.Utils;

namespace PartialArrays.Arrays.imp
{   
    unsafe public class PartialIntArray : IPartialArray<int>
    {
        private readonly int _count;
        
        private readonly int _fullNumberCount;
        private readonly int _bytesForNotFullNumber;
        
        private byte[] _controlledBytes;
        
        public PartialIntArray(double elementsCount)
        {
            var remains = elementsCount % 0.25; // 25 - а divisor that helps us understand if we can divide a number into bytes.
            if (remains != 0)
            {
                throw new Exception("The indivisible length of the array");
            }

            _fullNumberCount = (int)elementsCount;
            _bytesForNotFullNumber = (int)((elementsCount - _fullNumberCount)/ 0.25);

            int _controlledByesCount = _fullNumberCount * BytesIn.Int32 + _bytesForNotFullNumber;
            _controlledBytes = new byte[_controlledByesCount];
            _count = _fullNumberCount + Convert.ToInt32(_bytesForNotFullNumber > 0);

            
        }

        public int Count { get { return _count; } }
        public int ControlledBytesLength { get { return _controlledBytes.Length; } }

        public int this[int index] 
        {
            get => GetElement(index);
            set => SetElement(index, value); 
        }


        public int GetElement(int id)
        {
            if (id > Count - 1)
            {
                throw new IndexOutOfRangeException();
            }

            byte[] numberBytes = new byte[BytesIn.Int32];
            int result;

            // if is full number
            if (id < _fullNumberCount)
            {
                var numberStartByte = id * BytesIn.Int32;
                

                Array.Copy(_controlledBytes, numberStartByte, numberBytes, 0, BytesIn.Int32);

                if (BinaryPrimitives.TryReadInt32LittleEndian(numberBytes, out result))
                {
                    return result;
                }
                return 0;
            }

            //if is partial number
            Array.Copy(_controlledBytes, _controlledBytes.Length - _bytesForNotFullNumber, 
                numberBytes, 0, _bytesForNotFullNumber);

            ulong lastKnownByteAddress;
            fixed (byte* lastKnownByte = &numberBytes[_bytesForNotFullNumber - 1])
            {
                lastKnownByteAddress = (ulong)lastKnownByte;
            }

            try
            {
                for (int i = 0; i < BytesIn.Int32; i++)
                {
                    if (i < _bytesForNotFullNumber)
                    {
                        continue;
                    }

                    ulong address = lastKnownByteAddress + (ulong)i;
                    numberBytes[i] = ReadByteFromAddress(address);
                }
            }
            catch (Exception)
            {
                return 0;
            }

            if (BinaryPrimitives.TryReadInt32LittleEndian(numberBytes, out result))
            {
                return result;
            }
            return 0;
        }

        public void SetElement(int id, int value)
        {
            if (id > Count - 1)
            {
                throw new IndexOutOfRangeException();
            }

            byte[] numberBytes = new byte[4]; 
            BinaryPrimitives.TryWriteInt32LittleEndian(numberBytes, value);

            // if is full number
            if (id > _fullNumberCount)
            {
                Array.Copy(numberBytes, 0, _controlledBytes, id * BytesIn.Int32 + 1, BytesIn.Int32);
            }
            Array.Copy(numberBytes, 0, _controlledBytes, id * BytesIn.Int32, _bytesForNotFullNumber);
        }

        unsafe byte ReadByteFromAddress(ulong address)
        {
            return *(byte*)address;
        }
    }
}
