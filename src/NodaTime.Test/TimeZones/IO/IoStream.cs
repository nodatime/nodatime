// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones.IO
{
    /// <summary>
    ///   Provides a simple, fized-size, pipe-like stream that has a writer and a reader.
    /// </summary>
    /// <remarks>
    ///   When the buffer fills up an exception is thrown and currently the buffer is fixed at 4096. The write
    ///   pointer does not wrap so only a total of 4096 bytes can ever be written to the stream. This is designed
    ///   for testing purposes and not real-world uses.
    /// </remarks>
    internal class IoStream
    {
        private readonly byte[] buffer = new byte[4096];
        private int readIndex;
        private Stream? readStream;
        private int writeIndex;
        private Stream? writeStream;

        /// <summary>
        ///   Returns the next byte from the stream if there is one.
        /// </summary>
        /// <returns>The next byte.</returns>
        /// <exception cref="InternalBufferOverflowException">There are no more bytes in the buffer.</exception>
        private byte GetByte()
        {
            if (readIndex >= writeIndex)
            {
                throw new IOException("IoStream buffer empty in GetByte()");
            }
            return buffer[readIndex++];
        }

        public void AssertEndOfStream()
        {
            Assert.AreEqual(readIndex, writeIndex);
        }

        public void AssertUnreadContents(byte[] expected)
        {
            Assert.AreEqual(expected.Length, writeIndex - readIndex);
            var actual = new byte[expected.Length];
            Array.Copy(buffer, readIndex, actual, 0, writeIndex - readIndex);
            Assert.AreEqual(expected, actual);
            readIndex = writeIndex;
        }

        /// <summary>
        ///   Returns a <see cref="Stream" /> that can be used to read from the buffer.
        /// </summary>
        /// <remarks>
        ///   This can only be called once for each instance i.e. only one reader is permitted per buffer.
        /// </remarks>
        /// <returns>The read-only <see cref="Stream" />.</returns>
        /// <exception cref="InvalidOperationException">A reader was already requested.</exception>
        public Stream GetReadStream()
        {
            if (readStream != null)
            {
                throw new InvalidOperationException("Cannot call GetReadStream() twice on the same object.");
            }
            readStream = new ReadStreamImpl(this);
            return readStream;
        }

        /// <summary>
        ///   Returns a <see cref="Stream" /> that can be used to write to the buffer.
        /// </summary>
        /// <remarks>
        ///   This can only be called once for each instance i.e. only one writer is permitted per buffer.
        /// </remarks>
        /// <returns>The write-only <see cref="Stream" />.</returns>
        /// <exception cref="InvalidOperationException">A writer was already requested.</exception>
        public Stream GetWriteStream()
        {
            if (writeStream != null)
            {
                throw new InvalidOperationException("Cannot call GetWriteStream() twice on the same object.");
            }
            writeStream = new WriteStreamImpl(this);
            return writeStream;
        }

        /// <summary>
        ///   Adds a byte to the buffer.
        /// </summary>
        /// <param name="value">The byte to add.</param>
        /// <exception cref="InternalBufferOverflowException">The buffer has been filled.</exception>
        private void PutByte(byte value)
        {
            if (writeIndex >= buffer.Length)
            {
                throw new IOException("Exceeded the IoStream buffer size of " + buffer.Length);
            }
            buffer[writeIndex++] = value;
        }

        /// <summary>
        ///   Resets the stream to be empty.
        /// </summary>
        internal void Reset()
        {
            writeIndex = 0;
            readIndex = 0;
        }

        #region Nested type: ReadStreamImpl
        /// <summary>
        ///   Provides a read-only <see cref="Stream" /> implementaion for reading from the buffer.
        /// </summary>
        private class ReadStreamImpl : Stream
        {
            private readonly IoStream ioStream;

            /// <summary>
            ///   Initializes a new instance of the <see cref="ReadStreamImpl" /> class.
            /// </summary>
            /// <param name="stream">The <see cref="ioStream" /> to read from.</param>
            public ReadStreamImpl(IoStream stream)
            {
                ioStream = stream;
            }

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports reading.
            /// </summary>
            /// <returns>
            ///   true if the stream supports reading; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanRead => true;

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
            /// </summary>
            /// <returns>
            ///   true if the stream supports seeking; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanSeek => false;

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports writing.
            /// </summary>
            /// <returns>
            ///   true if the stream supports writing; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanWrite => false;

            /// <summary>
            ///   When overridden in a derived class, gets the length in bytes of the stream.
            /// </summary>
            /// <returns>
            ///   A long value representing the length of the stream in bytes.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            /// <summary>
            ///   When overridden in a derived class, gets or sets the position within the current stream.
            /// </summary>
            /// <returns>
            ///   The current position within the stream.
            /// </returns>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            ///   When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
            /// </summary>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            public override void Flush()
            {
            }

            /// <summary>
            ///   When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
            /// </summary>
            /// <returns>
            ///   The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
            /// </returns>
            /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name = "offset" /> and (<paramref name = "offset" /> + <paramref name = "count" /> - 1) replaced by the bytes read from the current source. 
            /// </param>
            /// <param name="offset">The zero-based byte offset in <paramref name = "buffer" /> at which to begin storing the data read from the current stream. 
            /// </param>
            /// <param name="count">The maximum number of bytes to be read from the current stream. 
            /// </param>
            /// <exception cref="T:System.ArgumentException">The sum of <paramref name = "offset" /> and <paramref name = "count" /> is larger than the buffer length. 
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name = "buffer" /> is null. 
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name = "offset" /> or <paramref name = "count" /> is negative. 
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support reading. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override int Read(byte[] buffer, int offset, int count)
            {
                count = Math.Min(count, ioStream.writeIndex - ioStream.readIndex);
                for (int i = 0; i < count; i++)
                {
                    buffer[i + offset] = ioStream.GetByte();
                }
                return count;
            }

            /// <summary>
            ///   When overridden in a derived class, sets the position within the current stream.
            /// </summary>
            /// <returns>
            ///   The new position within the current stream.
            /// </returns>
            /// <param name="offset">A byte offset relative to the <paramref name = "origin" /> parameter. 
            /// </param>
            /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position. 
            /// </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   When overridden in a derived class, sets the length of the current stream.
            /// </summary>
            /// <param name="value">The desired length of the current stream in bytes. 
            /// </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
            /// </summary>
            /// <param name="buffer">An array of bytes. This method copies <paramref name = "count" /> bytes from <paramref name = "buffer" /> to the current stream. 
            /// </param>
            /// <param name="offset">The zero-based byte offset in <paramref name = "buffer" /> at which to begin copying bytes to the current stream. 
            /// </param>
            /// <param name="count">The number of bytes to be written to the current stream. 
            /// </param>
            /// <exception cref="T:System.ArgumentException">The sum of <paramref name = "offset" /> and <paramref name = "count" /> is greater than the buffer length. 
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name = "buffer" /> is null. 
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name = "offset" /> or <paramref name = "count" /> is negative. 
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support writing. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
        #endregion

        #region Nested type: WriteStreamImpl
        /// <summary>
        ///   Provides a write-only <see cref="Stream" /> implementaion for writing to the buffer.
        /// </summary>
        private class WriteStreamImpl : Stream
        {
            private readonly IoStream ioStream;

            /// <summary>
            ///   Initializes a new instance of the <see cref="WriteStreamImpl" /> class.
            /// </summary>
            /// <param name="stream">The <see cref="IoStream" /> to read from.</param>
            public WriteStreamImpl(IoStream stream)
            {
                ioStream = stream;
            }

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports reading.
            /// </summary>
            /// <returns>
            ///   true if the stream supports reading; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanRead => false;

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
            /// </summary>
            /// <returns>
            ///   true if the stream supports seeking; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanSeek => false;

            /// <summary>
            ///   When overridden in a derived class, gets a value indicating whether the current stream supports writing.
            /// </summary>
            /// <returns>
            ///   true if the stream supports writing; otherwise, false.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public override bool CanWrite => true;

            /// <summary>
            ///   When overridden in a derived class, gets the length in bytes of the stream.
            /// </summary>
            /// <returns>
            ///   A long value representing the length of the stream in bytes.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            /// <summary>
            ///   When overridden in a derived class, gets or sets the position within the current stream.
            /// </summary>
            /// <returns>
            ///   The current position within the stream.
            /// </returns>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            ///   When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
            /// </summary>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            public override void Flush()
            {
            }

            /// <summary>
            ///   When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
            /// </summary>
            /// <returns>
            ///   The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
            /// </returns>
            /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name = "offset" /> and (<paramref name = "offset" /> + <paramref name = "count" /> - 1) replaced by the bytes read from the current source. 
            /// </param>
            /// <param name="offset">The zero-based byte offset in <paramref name = "buffer" /> at which to begin storing the data read from the current stream. 
            /// </param>
            /// <param name="count">The maximum number of bytes to be read from the current stream. 
            /// </param>
            /// <exception cref="T:System.ArgumentException">The sum of <paramref name = "offset" /> and <paramref name = "count" /> is larger than the buffer length. 
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name = "buffer" /> is null. 
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name = "offset" /> or <paramref name = "count" /> is negative. 
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support reading. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   When overridden in a derived class, sets the position within the current stream.
            /// </summary>
            /// <returns>
            ///   The new position within the current stream.
            /// </returns>
            /// <param name="offset">A byte offset relative to the <paramref name = "origin" /> parameter. 
            /// </param>
            /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position. 
            /// </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   When overridden in a derived class, sets the length of the current stream.
            /// </summary>
            /// <param name="value">The desired length of the current stream in bytes. 
            /// </param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///   When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
            /// </summary>
            /// <param name="buffer">An array of bytes. This method copies <paramref name = "count" /> bytes from <paramref name = "buffer" /> to the current stream. 
            /// </param>
            /// <param name="offset">The zero-based byte offset in <paramref name = "buffer" /> at which to begin copying bytes to the current stream. 
            /// </param>
            /// <param name="count">The number of bytes to be written to the current stream. 
            /// </param>
            /// <exception cref="T:System.ArgumentException">The sum of <paramref name = "offset" /> and <paramref name = "count" /> is greater than the buffer length. 
            /// </exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name = "buffer" /> is null. 
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name = "offset" /> or <paramref name = "count" /> is negative. 
            /// </exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs. 
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">The stream does not support writing. 
            /// </exception>
            /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. 
            /// </exception>
            /// <filterpriority>1</filterpriority>
            public override void Write(byte[] buffer, int offset, int count)
            {
                if (count > ioStream.buffer.Length - ioStream.writeIndex)
                {
                    throw new InvalidOperationException("The I/O buffer is full");
                }
                for (int i = 0; i < count; i++)
                {
                    ioStream.PutByte(buffer[i + offset]);
                }
            }
        }
        #endregion
    }
}
