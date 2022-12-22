using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021
{
    internal class Day16 : Day
    {
        private string InputFile = "2021/inputs/16.txt"; 

        int PartASolution = 0; 

        //----------------------------------------------------------------------------------------------
        public override void ParseInput()
        {
        }

        //----------------------------------------------------------------------------------------------
        private int HexToByte( char c )
        {
            return (c <= '9') ? (c - '0') : (c - 'A' + 10); 
        }

        //----------------------------------------------------------------------------------------------
        private byte[] ParseInputStream( string line )
        {
            byte[] values = new byte[line.Length / 2]; 

            int idx = 0;
            for (int i = 0; i < line.Length; i += 2)
            {
                byte v = (byte)((HexToByte( line[i] ) << 4) | HexToByte(line[i + 1])); 
                values[idx] = v; 
                ++idx; 
            }

            return values; 
        }

        //----------------------------------------------------------------------------------------------
        internal struct ReadHead
        {
            public int ByteOffset;
            public int BitOffset;

            public int Subtract( ReadHead other )
            {
                return (ByteOffset - other.ByteOffset) * 8 + (BitOffset - other.BitOffset); 
            }
        }

        //----------------------------------------------------------------------------------------------
        int ReadBit( byte[] data, ref ReadHead head )
        {
            if (head.BitOffset == 8)
            {
                ++head.ByteOffset; 
                head.BitOffset = 0; 
            }

            byte mask = (byte)(1 << (7 - head.BitOffset)); 
            ++head.BitOffset; 

            return ((data[head.ByteOffset] & mask) > 0) ? 1 : 0; 
        }

        //----------------------------------------------------------------------------------------------
        int ReadBits( byte[] data, ref ReadHead head, int bitCount )
        {
            int value = 0; 
            for (int i = 0; i < bitCount; ++i)
            {
                value = (value << 1) | ReadBit( data, ref head ); 
            }

            return value; 
        }

        //----------------------------------------------------------------------------------------------
        Int64 ReadValue( byte[] data, ref ReadHead head )
        {
            Int64 ret = 0; 
            int chunk; 
            
            do
            {
                chunk = ReadBits( data, ref head, 5 ); 
                ret = (ret << 4) | (Int64)(chunk & 0x0f); 
            } while ((chunk & 0x10) != 0); 
           
            return ret; 
        }

        //----------------------------------------------------------------------------------------------
        Int64 ReadPacket( byte[] data, ref ReadHead head )
        {
            int version = ReadBits( data, ref head, 3 ); 
            PartASolution += version; // hax

            int type = ReadBits( data, ref head, 3 ); 
            if (type == 4)
            {
                return ReadValue( data, ref head ); 
            }
            else
            {
                int lenType = ReadBit( data, ref head ); 
                List<Int64> subValues = new List<Int64>(); 
                if (lenType == 0)
                {
                    // total length of the sub packet
                    int bitLength = ReadBits( data, ref head, 15 ); 

                    ReadHead start = head; 
                    while (head.Subtract(start) < bitLength)
                    {
                        subValues.Add( ReadPacket( data, ref head ) ); 
                    }
                }
                else
                {
                    int numSubPackets = ReadBits( data, ref head, 11 ); 
                    for (int i = 0; i < numSubPackets; ++i)
                    {
                        subValues.Add( ReadPacket( data, ref head ) ); 
                    }
                }

                return type switch
                {
                    0 => subValues.Sum(), 
                    1 => subValues.Aggregate( 1L, (x, y) => x * y ), 
                    2 => subValues.Min(), 
                    3 => subValues.Max(), 
                    5 => (subValues[0] > subValues[1]) ? 1L : 0L, 
                    6 => (subValues[0] < subValues[1]) ? 1L : 0L, 
                    7 => (subValues[0] == subValues[1]) ? 1L : 0L, 
                    _ => 0
                };
            }
        }

        //----------------------------------------------------------------------------------------------
        public override string RunA()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            byte[] data = ParseInputStream( lines[0] ); 
            
            PartASolution = 0; 

            ReadHead head = new ReadHead(); 
            ReadPacket( data, ref head ); 

            return PartASolution.ToString(); 
        }

        //----------------------------------------------------------------------------------------------
        public override string RunB()
        {
            List<string> lines = Util.ReadFileToLines(InputFile); 
            byte[] data = ParseInputStream( lines[0] ); 
            
            ReadHead head = new ReadHead(); 
            Int64 answer = ReadPacket( data, ref head ); 

            return answer.ToString(); 
        }
    }
}
