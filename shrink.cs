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
            byte[] file = File.ReadAllBytes("/storage/emulated/0/Edited/S.html");
           
            int n;
            List<Byte> compressed = Compress(file);
            List<Byte> data = EncodeLoop(new List<Byte>(file), out n);
          //  List<Byte>  idata = Decode(data);
            
            
			Console.WriteLine("File {0} ::\n Zip {1} ::\n Qsx  {2} :: ",
			file.Length,
			compressed.Count,
			data.Count
			);
		 //	Console.WriteLine(Encoding.UTF8.GetString(idata.ToArray()));
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
		
		public static int[] code=new int[]           {1,1,1,0};
		public static int[] codebits=new int[]       {1,2,3,3};
	    public static List<Byte> EncodeLoop(List<Byte> data, out int nloop)
	    {
	    	nloop = 0;
	    	List<Byte> lzw = Compress(data.ToArray());
	    	var ok=lzw.Count*0.4;
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
			while(a<lzwdata.Count)
			{
				try{
				int l=(int)(lzwdata[a]&3);
				int m=(int)(lzwdata[a]>>2)&3;
				int n=(int)(lzwdata[a]>>4)&3;
				int o=(int)(lzwdata[a]>>6)&3;
				
				writeValue(bytes,code[o],codebits[o]);
				writeValue(bytes,code[n],codebits[n]);
				writeValue(bytes,code[m],codebits[m]);
				writeValue(bytes,code[l],codebits[l]);
				a++;
				}catch(Exception ex){ex.ToString();}
			}
			
			while(Power[bytes]!=0)
			   writeBit(bytes,1);
			
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
			
			decoded=0;
			int zero=readBit(qsxdata);
			if(zero==0){
				q = readBit(qsxdata);
				if (q==0){
					q=readBit(qsxdata);
					if(q==0){
						decoded+=3;
					}else{
						decoded+=2;
					}
				}else{
					decoded+=1;
				}
			}
			
			decoded=decoded<<2;
			
			zero=readBit(qsxdata);
			if(zero==0){
				q = readBit(qsxdata);
				if (q==0){
					q=readBit(qsxdata);
					if(q==0){
						decoded+=3;
					}else{
						decoded+=2;
					}
				}else{
					decoded+=1;
				}
			}
			
			decoded=decoded<<2;
			
			zero=readBit(qsxdata);
			if(zero==0){
				q = readBit(qsxdata);
				if (q==0){
					q=readBit(qsxdata);
					if(q==0){
						decoded+=3;
					}else{
						decoded+=2;
					}
				}else{
					decoded+=1;
				}
			}
			
			decoded=decoded<<2;
			
			zero=readBit(qsxdata);
			if(zero==0){
				q = readBit(qsxdata);
				if (q==0){
					q=readBit(qsxdata);
					if(q==0){
						decoded+=3;
					}else{
						decoded+=2;
					}
				}else{
					decoded+=1;
				}
			}
			
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
                    //compressed.Add(Convert.ToByte((dictionary[w]>>8)&255));
            		compressed.Add(Convert.ToByte(dictionary[w]&255));
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
 
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
            {
            	//	compressed.Add(Convert.ToByte((dictionary[w]>>8)&255));
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
 
            string w = dictionary[compressed[0]];
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