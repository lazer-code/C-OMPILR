using System.IO;
using System.Linq;

namespace lInker
{
	class COFFObjectFile : IObjectFile
	{
		string path = "";
		public COFF_FILE_HEADER hdr{get; set;}
		public List<COFF_SECTION> sections{get; set;}
		
		public COFFObjectFile(string path)
		{
			this.hdr 	 	= new(0,0,0,0,0,0,0);
			this.sections 	= new();
			this.path 		= path;
		}

		public void parse()
		{
			byte[] file_header = new byte[20];
			
			ushort f_magic, f_nscns;
			uint f_timdat, f_symptr, f_nsyms;
			ushort f_opthdr,f_flags; 

			int offset = 0;

			using (FileStream fsSource = new FileStream(this.path, FileMode.Open, FileAccess.Read))
			{
    			// read the first 20 bytes of the file into file_header array
    			int n = fsSource.Read(file_header, 0, 20);
			}

			#region file header info extraction
			// extract info from the header, see https://learn.microsoft.com/en-us/windows/win32/debug/pe-format#coff-file-header-object-and-image
			f_magic = Helper.extractUshort(file_header, ref offset);
			f_nscns = Helper.extractUshort(file_header, ref offset);
			f_timdat = Helper.extractInt(file_header, ref offset);
			f_symptr = Helper.extractInt(file_header, ref offset);
			f_nsyms = Helper.extractInt(file_header, ref offset);
			f_opthdr = Helper.extractUshort(file_header, ref offset);
			f_flags = Helper.extractUshort(file_header, ref offset);
			#endregion

			#region file header info debugging output
			Console.WriteLine("magic : " + f_magic);			
			Console.WriteLine("number of sections : " + f_nscns);			
			Console.WriteLine("timestamp : " + f_timdat);			
			Console.WriteLine("symbol table location : " + f_symptr);
			Console.WriteLine("symbol count : " + f_nsyms);
			Console.WriteLine("optional header size : " + f_opthdr);
			Console.WriteLine("flags are : " + f_flags);
			#endregion

			// create a new header consisting of the correct info and set it
			this.hdr = new(f_magic, f_nscns, f_timdat, f_symptr, f_nsyms, f_opthdr, f_flags);
		}

		public string toString()
		{
			return "";
		}
	}
}