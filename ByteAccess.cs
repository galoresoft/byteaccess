using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GaloreSoft.IO
{
    public class ByteAccess
    {
        List<byte> _data;

        public int Length { get { return _data.Count; } }

        public int Offset(int index)
        {
            int r = 0;
            int offset = index % _data.Count;

            r = offset < 0 ? _data.Count - offset : offset;
            
            return r;
        }

        public int this[int index]
        {
            get
            {
                if (_data.Count == 0)
                    throw new ArgumentOutOfRangeException();

                int offset = Offset(index);
                return _data[offset];
            }

            set
            {
                if (_data.Count == 0)
                    throw new ArgumentOutOfRangeException();

                int position = 0;

                if (index > _data.Count)
                {
                    int delta = _data.Count - index;
                    AddRange(delta);
                    position = index;
                }
                else
                {
                    position = Offset(index);
                }

                _data[position] = (byte)value;
            }
        }

        public byte[] this[int index, int length]
        {
            get
            {
                int position = Offset(index);
                return _data.GetRange(position,length).ToArray();
            }
        }

        public void SetString(int index, int length, string str, params object[] args)
        {
            string tmp = string.Format(str, args);
            byte[] buffer = Encoding.ASCII.GetBytes(tmp);
            int size = buffer.Length;
            int wrote = 0;

            if (index + size > _data.Count)
            {
                int delta = (index + size) - _data.Count;
                AddRange(delta);
            }

            for (int i = 0; i < size; i++)
            {
                if (wrote >= length)
                    break;

                _data[index + i] = buffer[i];
                wrote++;
            }
        }
        
        
        public ByteAccess(int size = 0)
        {
            _data = new List<byte>();
            AddRange(size);
        }

        public ByteAccess(string filename)
        {
            _data = File.ReadAllBytes(filename).ToList();
        }

        public void AddRange(int size)
        {
            for (int i = 0; i < size; i++)
            {
                _data.Add(0);
            }
        }


        public void FillZeros(int index, int length)
        {

        }

        public string GetASCIIString(int offset, int length)
        {
            return ASCIIEncoding.ASCII.GetString(this[offset, length]).Trim();
        }
        
        public int GetInt16(int offset)
        {
            return BitConverter.ToInt16(this[offset,2], 0);
        }

        public int GetInt32(int offset)
        {
            return BitConverter.ToInt32(this[offset,4], 0);
        }

        public void SaveOffset(string filename, int offset, int lenght)
        {
            byte[] buffer = this[offset, lenght];
            File.WriteAllBytes(filename,buffer);
        }
    }
}
