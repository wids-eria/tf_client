using System;
using System.IO;

public static class StreamExtensions {
	public static int	PeekByte( this Stream self ) {
		if( !self.CanSeek ) {
			throw new NotSupportedException();
		}
		
		long pos = self.Position;
		int b = self.ReadByte();
		self.Seek( pos, SeekOrigin.Begin );
		return b;
	}
}