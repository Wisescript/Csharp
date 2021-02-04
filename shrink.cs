using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace LZW
{
    public class Program
    {
        public static void Main(string[] args)
        {
        	Console.WriteLine("Encoding...");
            
            List<int> compressed = Compress(File.ReadAllBytes("/storage/emulated/0/Download/fresh.mp3"));
            List<Byte> data = Qsx(compressed);
            List<int> lzw = Compress(data.ToArray());
			//while(lzw.Count>100)
			//{
			//  data = Qsx(lzw);
			//  lzw = Compress(data.ToArray());
			//  Console.WriteLine(lzw.Count);
			//}
			Console.WriteLine("{0} :: {1} :: {2} ", compressed.Count,data.Count,lzw.Count);
            
            string decompressed = Decompress(compressed);
            //Console.WriteLine(decompressed);
        }
		public static void write(List<Byte>output, int data, int bits)
		{
			while(bits>0)
			{
				bits--;
				writeBit(output,((data>>bits)&1));
			}
		}
		
		public static Dictionary<object,int> Bits=new Dictionary<object,int>();
		public static Dictionary<object,int> Power=new Dictionary<object,int>();
		
		public static void writeBit(List<Byte>output, int data)
		{
			if(Power.ContainsKey(output))
			Power[output]++;
			else
			Power[output]=1;
			
            if (Bits.ContainsKey(output))
			Bits[output]+=(data==0?0:1)<<(8-Power[output]);
			else
			Bits[output]=(data==0?0:1)<<(8-Power[output]);
			
			if (Power[output]>=8)
			{
				output.Add((Byte)Bits[output]);
				Bits[output]=0;
				Power[output]=0;
			}
		}
		public static List<int>code=new List<int>    {1,1,2,3,4,5,6,7,1,2,3,4,5,6,7,0};
		public static List<int>codebits=new List<int>{1,4,4,4,4,4,4,4,8,8,8,8,8,8,8,8};
		
		public static List<Byte> Qsx(List<int> lzwdata)
		{
			var bytes = new List<Byte>();
Bits=new Dictionary<object,int>();
Power=new Dictionary<object,int>();
		
			var a=0;
			while(a<lzwdata.Count)
			{
				try{
				int l=(int)(lzwdata[a++]&15);
				int m=(int)(lzwdata[a++]>>4)&15;
				int n=(int)(lzwdata[a++]>>8)&15;
				int o=(int)(lzwdata[a++]>>12)&15;
				
				write(bytes,code[l],codebits[l]);
				write(bytes,code[m],codebits[m]);
				write(bytes,code[n],codebits[n]);
				write(bytes,code[o],codebits[o]);
				
				}catch(Exception ex){ex.ToString();}
			}
				  
				
			
			return bytes;
		}
		public static List<int> Usx(List<Byte> qsxdata)
		{
			var ints=new List<int>();
			return ints;
		}
        public static List<int> Compress(byte[] uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);
 
            string w = string.Empty;
            List<int> compressed = new List<int>();
 
            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
 
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);
 
            return compressed;
        }
 
        public static string Decompress(List<int> compressed)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());
 
            string w = dictionary[compressed[0]];
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);
 
            foreach (int k in compressed)
            {
                string entry = null;
                if (dictionary.ContainsKey(k))
                    entry = dictionary[k];
                else if (k == dictionary.Count)
                    entry = w + w[0];
 
                decompressed.Append(entry);
 
                // new sequence; add it to the dictionary
                dictionary.Add(dictionary.Count, w + entry[0]);
 
                w = entry;
            }
 
            return decompressed.ToString();
        }
    }
}