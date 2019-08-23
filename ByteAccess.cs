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
                int position = Fill(index);
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

        public int Fill(int index)
        {
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

            return position;
        }

        public int Fill(int index, IEnumerable<byte> data, int length = -1)
        {
            int position = 0;
            int s = _data.Count;
            int n = data.Count();

            int j = 0;

            for (int i = 0; i < n; i++)
            {
                if (length != -1 && j > length)
                    break;

                if ((index + i) < s)
                {
                    _data[index + i] = data.ElementAt(j);
                }
                else
                {
                    _data.Add(data.ElementAt(j));
                }

                j++;
            }

            return position;
        }

        public int Append(IEnumerable<byte> data)
        {
            _data.AddRange(data);
            return _data.Count - 1;
        }

        public int Append(byte data)
        {
            _data.Add(data);
            return _data.Count - 1;
        }

        public int Append(ByteAccess origin)
        {
            _data.AddRange(origin._data);
            return _data.Count - 1;
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

        public int AddRange(int size, byte data = 0)
        {

            for (int i = 0; i < size; i++)
            {
                _data.Add(0);
            }

            return _data.Count - 1;
        }
                
        public string GetASCIIString(int offset, int length)
        {
            return ASCIIEncoding.ASCII.GetString(this[offset, length]).Trim();
        }
        
        public Int16 GetInt16(int offset)
        {
            return BitConverter.ToInt16(this[offset,2], 0);
        }

        public Int32 GetInt32(int offset)
        {
            return BitConverter.ToInt32(this[offset,4], 0);
        }

        public UInt16 GetUInt16(int offset)
        {
            return BitConverter.ToUInt16(this[offset, 2], 0);
        }

        public UInt32 GetUInt32(int offset)
        {
            return BitConverter.ToUInt32(this[offset, 4], 0);
        }

        public void SetByte(int index, int value)
        {
            int position = Fill(index);
            _data[position] = (byte)value;
        }

        public void SetBytes(int index, int lenght, IEnumerable<byte> data)
        {
            int position = Fill(index + lenght);
        }

        public void SetByte(int index, byte value)
        {
            int position = Fill(index);
            _data[position] = value;
        }

        public void SetUInt16(int index, UInt16 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            int position = Fill(index, buffer);
        }

        public void SetUInt32(int index, UInt32 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            int position = Fill(index, buffer);
        }

        public void SetASCII(int index, int length, string str, params object[] args)
        {
            string tmp = string.Format(str, args);
            byte[] buffer = Encoding.ASCII.GetBytes(tmp);

            Fill(index, buffer, length);
        }

        public byte[] GetBuffer()
        {
            return _data.ToArray();
        }

        public void Save(string filename)
        {
            File.WriteAllBytes(filename, _data.ToArray());
        }

        public void SaveOffset(string filename, int offset, int lenght)
        {
            byte[] buffer = this[offset, lenght];
            File.WriteAllBytes(filename,buffer);
        }
    }
}
