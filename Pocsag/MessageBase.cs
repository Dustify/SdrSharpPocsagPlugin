namespace Pocsag
{
    using System;
    using System.Security.Cryptography;

    public enum MessageType
    {
        AlphaNumeric,
        Numeric,
        Tone
    }

    public class MessageBase
    {
        public DateTime Timestamp { get; set; }

        public string Protocol { get; set; }

        public string TimestampText { get; set; }

        public string Address { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorText { get; set; }

        public bool HasData { get; set; }

        public string Hash { get; set; }

        public string Payload { get; set; }

        public MessageType Type { get; set; }

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
                this.Timestamp = DateTime.Now;
                this.TimestampText = $"{this.Timestamp.ToShortDateString()} {this.Timestamp.ToLongTimeString()}";
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

            var sha256 = SHA256.Create();

            var hashBytes = sha256.ComputeHash(bytesToHash);

            sha256.Dispose();
            sha256 = null;

            this.Hash = BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
