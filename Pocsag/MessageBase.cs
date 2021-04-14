namespace Pocsag
{
    using System;
    using System.Collections.Generic;

    public enum MessageType
    {
        AlphaNumeric,
        Numeric,
        Tone
    }

    public abstract class MessageBase
    {
        public DateTime Timestamp { get; }

        public int FrameIndex { get; protected set; }

        public UInt32 Address { get; protected set; }

        public byte Function { get; protected set; }

        public string TimestampText => $"{this.Timestamp.ToShortDateString()} {this.Timestamp.ToLongTimeString()}";

        public bool HasData { get; protected set; }

        public uint Bps { get; }

        public string Hash { get; private set; }

        public List<bool> RawPayload { get; private set; }

        public string Payload { get; protected set; }

        public MessageType Type { get; protected set; }

        public string TypeText
        {
            get
            {
                switch (this.Type)
                {
                    case MessageType.AlphaNumeric:
                        return "Alpha";
                    case MessageType.Numeric:
                        return "Numeric";
                    case MessageType.Tone:
                        return "Tone";
                }

                return string.Empty;
            }
        }

        public MessageBase(uint bps)
        {
            try
            {
                this.Bps = bps;
                this.RawPayload = new List<bool>();

                this.Timestamp = DateTime.Now;
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        protected void UpdateHash()
        {
            var textToHash = this.Payload;

            // skip first 9 characters, typically contains time / date + another number which will mess up duplicate detection

            if (textToHash.Length > 9)
            {
                textToHash = textToHash.Substring(8);
            }

            var bytesToHash = System.Text.Encoding.ASCII.GetBytes(textToHash);

            var sha256 = new System.Security.Cryptography.SHA256Managed();

            var hashBytes = sha256.ComputeHash(bytesToHash);

            sha256.Dispose();
            sha256 = null;

            this.Hash = BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
