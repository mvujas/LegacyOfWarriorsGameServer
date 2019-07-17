﻿using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace Utils.Net
{
    internal class MessageReceiver
    {
        public MessageReceiver(AsyncUserToken userToken)
        {
            m_userToken = userToken ?? 
                throw new ArgumentNullException(nameof(userToken));
        }

        public Action<AsyncUserToken> OnMessageError { set; private get; }
        public Action<MessageWrapper> OnMessageFullyReceived { set; private get; }

        private AsyncUserToken m_userToken;

        private int m_bytesToReceive = 0;
        private byte[] m_messageBuffer = null;

        const int PREFIX_SIZE = 4;

        private int m_bytesLeftForPrefix = PREFIX_SIZE;
        private byte[] m_lengthPrefixArr = new byte[PREFIX_SIZE];


        private object m_receivingMessageLock = new object();

        static string bytesToStr(byte[] bajtovi)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var bajt in bajtovi)
            {
                stringBuilder.Append(bajt);
            }
            return stringBuilder.ToString();
        }

        public void Process(byte[] message, int messageBytes)
        {
            Console.WriteLine("Buffer size: " + message.Length);
            Console.WriteLine("Primac trazi katanac. poruka duzine: " + BitConverter.ToInt32(message, 0));
            lock(m_receivingMessageLock)
            {
                try
                {
                    Console.WriteLine("Bajtovi: " + bytesToStr(message));
                    ContinueReceive(message, 0, messageBytes);
                }
                catch(Exception)
                {
                    ResetAndReturnError();
                }
            }
        }

        private void ContinueReceive(byte[] message, int offset, int length)
        {
            int start, bytesToGet;
            if(m_messageBuffer == null)
            {
                int prefixArrStartIndex = PREFIX_SIZE - m_bytesLeftForPrefix;
                int readBytes = Math.Min(length, m_bytesLeftForPrefix);
                Array.Copy(message, offset, m_lengthPrefixArr, prefixArrStartIndex, readBytes);
                m_bytesLeftForPrefix -= readBytes;

                if(m_bytesLeftForPrefix > 0)
                {
                    return;
                }

                int length_prefix = BitConverter.ToInt32(m_lengthPrefixArr, 0);

                Console.WriteLine($" === NOVA PORUKA DUZINE: {length_prefix} ===");

                m_bytesToReceive = length_prefix;
                m_messageBuffer = new byte[length_prefix];
                start = offset + readBytes;
                bytesToGet = Math.Min(length - readBytes, length_prefix);
            }
            else
            {
                start = offset;
                bytesToGet = Math.Min(length, m_bytesToReceive);
            }
            int firstEmptyIndex = m_messageBuffer.Length - m_bytesToReceive;
            Array.Copy(message, start, m_messageBuffer, firstEmptyIndex, bytesToGet);
            m_bytesToReceive -= bytesToGet;
            Console.WriteLine("Left to receive: " + m_bytesToReceive);
            if (m_bytesToReceive == 0)
            {
                ProcessFullyReceivedMessage();
            }

            int bytesLeftInMessage = offset + length - (start + bytesToGet);
            if(bytesLeftInMessage > 0)
            {
                ContinueReceive(message, start + bytesToGet, bytesLeftInMessage);
            }
        }

        private void Reset()
        {
            m_bytesToReceive = 0;
            m_messageBuffer = null;
            m_bytesLeftForPrefix = PREFIX_SIZE;
        }

        private void ResetAndReturnError()
        {
            Reset();
            OnMessageError(m_userToken);
        }

        private void ProcessFullyReceivedMessage()
        {
            MessageWrapper messageWrapper = new MessageWrapper
            {
                Message = m_messageBuffer, 
                UserToken = m_userToken
            };
            Reset();
            OnMessageFullyReceived(messageWrapper);
        }
    }
}
