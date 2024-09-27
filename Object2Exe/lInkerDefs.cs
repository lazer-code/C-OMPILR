namespace lInker
{
	struct COFF_FILE_HEADER
	{
		ushort 		f_magic;		/* Magic number */	
		ushort 		f_nscns;		/* Number of Sections */
		uint 		f_timdat;		/* Time & date stamp */
		uint 		f_symptr;		/* File pointer to Symbol Table */
		uint 		f_nsyms;		/* Number of Symbols */
		ushort 		f_opthdr;		/* sizeof(Optional Header) */
		ushort 		f_flags;		/* Flags */

		public COFF_FILE_HEADER(ushort magic, ushort nscns, uint timdat, uint symptr, uint nsyms, ushort opthdr, ushort flags)
		{
			f_magic 	= magic;
			f_nscns 	= nscns;
			f_timdat  	= timdat;
			f_symptr 	= symptr;
			f_nsyms 	= nsyms;
			f_opthdr 	= opthdr;
			f_flags 	= flags;
		}
	}

	struct COFF_OPT_HEADER
	{
		ushort 		magic;          /* Magic Number                    */
		ushort 		vstamp;         /* Version stamp                   */
		ulong 		tsize;          /* Text size in bytes              */
		ulong 		dsize;          /* Initialised data size           */
		ulong 		bsize;          /* Uninitialised data size         */
		ulong 		entry;          /* Entry point                     */
		ulong 		text_start;     /* Base of Text used for this file */
		ulong 		data_start;     /* Base of Data used for this file */
	}

	struct COFF_SECTION_HEADER
	{
		char[]		s_name;		/* Section Name */
		long		s_paddr;	/* Physical Address */
		long		s_vaddr;	/* Virtual Address */
		long		s_size;		/* Section Size in Bytes */
		long		s_scnptr;	/* File offset to the Section data */
		long		s_relptr;	/* File offset to the Relocation table for this Section */
		long		s_lnnoptr;	/* File offset to the Line Number table for this Section */
		ushort		s_nreloc;	/* Number of Relocation table entries */
		ushort		s_nlnno;	/* Number of Line Number table entries */
		long		s_flags;	/* Flags for this section */
	}

	struct SYM_TREE_ENTRY
	{
		char[]		n_name;	    /* Symbol Name */
		long		n_value;	/* Value of Symbol */
		short		n_scnum;	/* Section Number */
		ushort		n_type;		/* Symbol Type */
		char		n_sclass;	/* Storage Class */
		char		n_numaux;	/* Auxiliary Count */
	}


	struct COFF_SECTION
	{
		COFF_SECTION_HEADER hdr; 		/* The header containing all needed info for section parsing */
		List<string> 		content; 	/* the content of the section */
	}

	interface IObjectFile
	{
		// variables
		COFF_FILE_HEADER hdr{get; set;}
		List<COFF_SECTION> sections{get; set;}

		// functions
		public abstract void parse();
		public abstract string toString();
	}
}