using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace dotNet
{
	
    public class QSX
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Encoding...");
            Bits = new Dictionary<object,int>();
            Power= new Dictionary<object,int>();
         //   byte[] file = File.ReadAllBytes("/storage/emulated/0/Edited/L.jpeg");
            Byte[] file = Encoding.UTF8.GetBytes("qsx for csharp v 0.02");
            int n;
            List<Byte> compressed = Compress(file);
            String text = Decompress(compressed);
            
            List<Byte> data = Encode(new List<Byte>(file));
            List<Byte>  idata = Decode(data);
            
            
			Console.WriteLine("File {0} ::\n Zip {1} ::\n Qsx  {2} :: ",
			file.Length,
			compressed.Count,
			data.Count
			);
		 //	Console.WriteLine(Encoding.UTF8.GetString(idata.ToArray()));
		   // Console.WriteLine(text);
		 }
		public static int[] merge(List<Byte>bytes,int []x ,int []y)
		{
			int[] r=new int[x.Length+y.Length];
			int i=0,j=0,k=0;
			while(i<x.Length && j<y.Length)
			  if(x[i]<y[j])
			  {
			  	r[k++]=x[i++];
			  	writeBit(bytes,1);
			  }else{
			  	r[k++]=y[j++];
			  	writeBit(bytes,0);
			  }
			  while(i<x.Length)
			    r[k++]=x[i++];
			  while(j<y.Length)
			    r[k++]=y[j++];
			   return r;
		}
		public static int[] merge_sort(List<Byte> bytes, int[] items,int o, int e)
		{
			int []a;
			int []b;
			int m = (e-o)/2+e;
			
			if ((e-m)>1)
			{
				a=merge_sort(bytes,items,o,m);
				b=merge_sort(bytes,items,m,e);
				return merge(bytes,a,b);
			}
			a=new int[]{items[e]};
			b=new int[]{items[o]};
			return merge(bytes,a,b);
		}
		public static int[] merge_read(List<Byte>bytes, int []x, int [] y)
		{
			int[] r=new int[x.Length+y.Length];
			int i=0,j=0,k=0;
			while(i<x.Length && j<y.Length)
			  if(readBit(bytes)==1)
			  {
			  	r[k++]=x[i++];
			  }else{
			  	r[k++]=y[j++];
			  }
			  while(i<x.Length)
			    r[k++]=x[i++];
			  while(j<y.Length)
			    r[k++]=y[j++];
			   return r;
			
			
			return x;
		}
		public static int[] merge_sort_read(List<Byte>bytes, int [] items, int o, int e)
		{
			
			
			int []a;
			int []b;
			int m = (e-o)/2+e;
			
			if ((e-m)>1)
			{
				a=merge_sort_read(bytes,items,o,m);
				b=merge_sort_read(bytes,items,m,e);
				return merge_read(bytes,a,b);
			}
			a=new int[]{items[e]};
			b=new int[]{items[o]};
			return merge_read(bytes,a,b);
			
			
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
				
			}catch(Exception ex){ex.ToString(); }
			return (value==0)?0:1;
		}
		
         public static List<Byte> EncodeLoop(List<Byte> data, out int nloop)
	    {
	    	nloop = 0;
	    	List<Byte> lzw = Compress(data.ToArray());
	    	var ok=300;
            while(lzw.Count>ok)
            {
              var qsxdata = Encode(lzw);
              lzw = Compress(qsxdata.ToArray());
              
              nloop++;
            }
            return lzw;
	    }
		public static List<Byte> Encode(List<Byte> lzwdata)
		{
			var bytes = new List<Byte>();
			Bits[lzwdata]=0;
			Power[lzwdata]=0;
		
			var a=0;
			var max=0;
			
			var sorted = new int[256];
			var initial = new int[128];
			while(a<lzwdata.Count)
			{
				for(int i=0;i<70 && (a+i)<lzwdata.Count;i++)
				{
					if (max<lzwdata[a])max=lzwdata[a];
					initial[i]=lzwdata[a];
					sorted[lzwdata[a++]]++;
				}
				
				for(int i=0;i<max;i++){
				  while(sorted[i]!=0){writeBit(bytes,1);sorted[i]--; }
				  writeBit(bytes,0);}
				  
				merge_sort(bytes,initial,0,69);
			}
			
			while(Power[bytes]!=0)
			   writeBit(bytes,0);
			
			return bytes;
		}
		public static List<Byte> Decode(List<Byte> qsxdata)
		{
			var ints=new List<Byte>();
			
			Bits[qsxdata]=0;
			Power[qsxdata]=0;
			
			int i,q,decoded=0;
		
			
			while(Bits[qsxdata]<(qsxdata.Count))
			{
				/*todo*/
				readBit(qsxdata);
				ints.Add(Convert.ToByte(decoded));
			}
			
			return ints;
		}
        public static List<Byte> Compress(byte[] uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(((char)i).ToString(), i);
 
            string w = string.Empty;
            List<Byte> compressed = new List<Byte>();
			
			
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
                    compressed.Add(Convert.ToByte((dictionary[w]>>8)&255));
            		compressed.Add(Convert.ToByte(dictionary[w]&255));
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
 
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
            {
            		compressed.Add(Convert.ToByte((dictionary[w]>>8)&255));
            		compressed.Add(Convert.ToByte(dictionary[w]&255));
            }
            return compressed;
        }
 
        public static string Decompress(List<Byte> compressed)
        {
            // build the dictionary
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());
 
           string w = dictionary[(compressed[0]<<8)+compressed[1]];
            compressed.RemoveAt(0);
            compressed.RemoveAt(0);
            StringBuilder decompressed = new StringBuilder(w);
 
            for (int c=0; c< compressed.Count;c++)
            {
            	int k=(compressed[c++]<<8)+compressed[c];
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