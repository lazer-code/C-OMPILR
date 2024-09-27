namespace lInker
{
	class Helper
	{
		public static uint extractInt(byte[] src, ref int offset)
		{
			uint temp = BitConverter.ToUInt32(src.Skip(offset).Take(Constants.LONG_SIZE).ToArray(), 0);
			offset += Constants.INT_SIZE;
			return temp;
		}

		public static ushort extractUshort(byte[] src, ref int offset)
		{
			ushort temp = BitConverter.ToUInt16(src.Skip(offset).Take(Constants.SHORT_SIZE).ToArray(), 0);
			offset += Constants.SHORT_SIZE;
			return temp;
		}
	}
}