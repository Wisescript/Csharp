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
            Bits = new Dictionary<object,int>();
            Power= new Dictionary<object,int>();
            List<int> compressed = Compress(File.ReadAllBytes("/storage/emulated/0/Edited/L.html"));
           
            //todo:debug
            List<Byte> data = Qsx(compressed);
            List<int> idata = Usx(data);
            
            Console.WriteLine(String.Join(",",compressed));
			Console.WriteLine("::");
			Console.WriteLine(String.Join(",",idata));
			
            //string decompressed = Decompress(idata);
            //Console.WriteLine(decompressed);
        }
		public static void writeValue(List<Byte>output, int data, int bits)
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
		public static int readValue(List<Byte> input, int bits)
		{
			int value=0;
			while(bits>0)
			{
				bits--;
				value=value<<1;
				value+=readBit(input);
			}
			return value;
		}
		public static int readBit(List<Byte> input)
		{
			int value=0;
			try{
			if(Power.ContainsKey(input))
			Power[input]++;
			else
			Power[input]=1;
			
			if (!Bits.ContainsKey(input))
			  Bits[input]=0;
			
			
			value=((input[Bits[input]])>>(8-Power[input]))&1;
			
			if (Power[input]>=8)
			{
			   Bits[input]++;
			   Power[input]=0;
			}
				
			}catch(Exception ex){ex.ToString();}
			return (value==0)?0:1;
		}
		
		public static int[] code=new int[]           {1,1,2,3,4,5,6,7,1,2,3,4,5,6,7,0};
		public static int[] codebits=new int[]       {1,4,4,4,4,4,4,4,8,8,8,8,8,8,8,8};
	    public static int[] decode=new int[]  {15,8,9,10,11,12,13,14};
		
		public static List<Byte> Qsx(List<int> lzwdata)
		{
			var bytes = new List<Byte>();
			
		
			var a=0;
			while(a<lzwdata.Count)
			{
				try{
				int l=(int)(lzwdata[a]&15);
				int m=(int)(lzwdata[a]>>4)&15;
				int n=(int)(lzwdata[a]>>8)&15;
				int o=(int)(lzwdata[a]>>12)&15;
				
				writeValue(bytes,code[o],codebits[o]);
				writeValue(bytes,code[n],codebits[n]);
				writeValue(bytes,code[m],codebits[m]);
				writeValue(bytes,code[l],codebits[l]);
				a++;
				}catch(Exception ex){ex.ToString();}
			}
				  
				
			
			return bytes;
		}
		public static List<int> Usx(List<Byte> qsxdata)
		{
			var ints=new List<int>();
			
			int i,q,decoded=0;
			int zero=readBit(qsxdata);
			
		while(Bits[qsxdata]<qsxdata.Count)
		{
			
			
			if(zero==0){
				q = readValue(qsxdata,3);
				if (q==0){
					q=readValue(qsxdata,4);
					decoded+=decode[q];
				}else{
					decoded+=q;
				}
			}
			
			decoded=decoded<<4;
			
			zero=readBit(qsxdata);
			
			if(zero==0){
				q = readValue(qsxdata,3);
				if (q==0){
					q=readValue(qsxdata,4);
					decoded+=decode[q];
				}else{
					decoded+=q;
				}
			}
			
			decoded=decoded<<4;
			
			zero=readBit(qsxdata);
			
			if(zero==0){
				q = readValue(qsxdata,3);
				if (q==0){
					q=readValue(qsxdata,4);
					decoded+=decode[q];
				}else{
					decoded+=q;
				}
			}
			
			decoded=decoded<<4;
			
			zero=readBit(qsxdata);
			if(zero==0){
				q = readValue(qsxdata,3);
				if (q==0){
					q=readValue(qsxdata,4);
					decoded+=decode[q];
				}else{
					decoded+=q;
				}
			}
			
			ints.Add(decoded);
			
			zero=readBit(qsxdata);
		}
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