// Message factory exception.
// Copyright (C) 2008-2010 Malcolm Crowe, Lex Li, and other contributors.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 9/6/2009
 * Time: 4:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Runtime.Serialization;
#if !NETFX_CORE
//using System.Security.Permissions;
#endif
namespace SharpSnmpLib.Messaging
{
    /// <summary>
    /// Message factory exception.
    /// </summary>
    [DataContract]
    public sealed class MessageFactoryException : SnmpException
    {
        private byte[] _bytes;
        
        /// <summary>
        /// Creates a <see cref="MessageFactoryException"/>.
        /// </summary>
        public MessageFactoryException()
        {
        }
        
        /// <summary>
        /// Creates a <see cref="MessageFactoryException"/> instance with a specific <see cref="String"/>.
        /// </summary>
        /// <param name="message">Message</param>
        public MessageFactoryException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Creates a <see cref="MessageFactoryException"/> instance with a specific <see cref="String"/> and an <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public MessageFactoryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>        
        public byte[] GetBytes()
        {
            return _bytes; 
        }
        
        /// <summary>
        /// Sets the bytes.
        /// </summary>
        /// <param name="value">Bytes.</param>
        public void SetBytes(byte[] value)
        {
            _bytes = value;
        }
        
        /// <summary>
        /// Returns a <see cref="String"/> that represents this <see cref="MessageFactoryException"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SharpMessageFactoryInnerException: {0}", Message);
        }
    }
}
