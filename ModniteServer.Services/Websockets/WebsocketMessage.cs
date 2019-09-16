﻿using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModniteServer.Websockets
{
    public enum MessageType
    {
        ContinuationFrame,
        Text,
        Binary,
        Ping,
        Pong
    }

    public sealed class WebsocketMessage
    {
        internal WebsocketMessage(byte[] buffer)
        {
            IsCompleted = (buffer[0] & 0b_1000_0000) != 0;

            byte opcode = (byte)(buffer[0] & 0b_0000_1111);
            bool maskBit = (buffer[1] & 0b_1000_0000) != 0;
            int payloadLength = buffer[1] - 128;

            if (!maskBit)
            {
                // Invalid client message (all client messages must be masked)
                //Log.Warning("Invalid Websocket message received from client");
            }

            int maskOffset = 2;
            if (payloadLength == 126)
            {
                payloadLength = (buffer[2] << 8) | buffer[3];
                maskOffset += 2;
            }
            else if (payloadLength == 127)
            {
                payloadLength = (buffer[2] << 24) | (buffer[3] << 16) | (buffer[4] << 8) | buffer[5];
                maskOffset += 4;
            }

            byte[] mask = BitConverter.GetBytes(BitConverter.ToInt32(buffer, maskOffset));

            byte[] decoded = new byte[payloadLength];
            for (int i = 0; i < payloadLength; i++)
            {
                decoded[i] = (byte)(buffer[i + 4 + maskOffset] ^ mask[i % 4]);
            }

            switch (opcode)
            {
                case 0:
                    MessageType = MessageType.ContinuationFrame;
                    break;

                case 1:
                    MessageType = MessageType.Text;
                    TextContent = Encoding.UTF8.GetString(decoded);
                    break;

                case 2:
                    MessageType = MessageType.Binary;
                    BinaryContent = decoded;
                    break;

                case 8: // close
                    // todo
                    break;

                case 9: // ping
                    // todo
                    break;

                case 10: // pong
                    // todo
                    break;

                default:
                    Log.Warning("Invalid opcode received");
                    break;
            }
        }

        internal WebsocketMessage()
        {
        }

        public MessageType MessageType { get; set; }

        public bool IsCompleted { get; set; }

        public string TextContent { get; set; }

        public byte[] BinaryContent { get; set; }

        internal static WebsocketMessage Defragment(IEnumerable<WebsocketMessage> fragments)
        {
            // TODO
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                if (MessageType == MessageType.Text)
                {
                    writer.Write((byte)0b_1000_0001);

                    byte[] textBuffer = Encoding.UTF8.GetBytes(TextContent);
                    if (textBuffer.Length >= 126)
                    {
                        if (textBuffer.Length > ushort.MaxValue)
                        {
                            writer.Write((byte)0b_0111_1111);

                            // TODO: big endian
                            //writer.Write(textBuffer.Length);
                            throw new NotImplementedException($"Text content larger than {ushort.MaxValue} is not supported yet");
                        }
                        else
                        {
                            writer.Write((byte)0b_0111_1110);
                            writer.Write((byte)((textBuffer.Length & 0xFF00) >> 8));
                            writer.Write((byte)(textBuffer.Length & 0xFF));
                        }
                    }
                    else
                    {
                        writer.Write((byte)textBuffer.Length);
                    }

                    writer.Write(textBuffer);
                }
                else if (MessageType == MessageType.Binary)
                {
                    if (BinaryContent.Length >= 0b_0111_1110)
                    {
                        throw new NotImplementedException("Binary content larger than 127 bytes is not supported yet");
                    }

                    writer.Write((byte)0b_1000_0010);
                    writer.Write((byte)BinaryContent.Length);
                    writer.Write(BinaryContent);
                }
                else
                {
                    throw new NotImplementedException("Other message types are not supported yet");
                }

                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            if (MessageType == MessageType.Text)
            {
                return TextContent;
            }
            else if (MessageType == MessageType.Binary)
            {
                return BitConverter.ToString(BinaryContent).Replace("-", " ");
            }
            else
            {
                return nameof(WebsocketMessage);
            }
        }
    }
}
