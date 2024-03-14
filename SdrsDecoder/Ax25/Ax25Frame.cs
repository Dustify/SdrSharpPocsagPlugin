using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SdrsDecoder
{
    public struct Ax25Address
    {
        public string Call;
        public byte Ssid;
        public byte RR;
        public byte CRH;
    }

    public enum Ax25ControlType
    {
        None,
        IShort,
        SShort,
        UShort,
        // ILong,
        // SLong
    }

    public class Ax25Frame
    {
        private int address_counter = 0;

        public List<Ax25Address> Addresses { get; } = new List<Ax25Address>();

        public Ax25Address CurrentAddress = new Ax25Address();

        public Ax25ControlType ControlType { get; private set; } = Ax25ControlType.None;

        public byte ControlNsssmm { get; private set; }

        public byte ControlPf { get; private set; }

        public byte ControlNrmmm { get; private set; }

        public bool AddAddressByte(byte value)
        {
            this.address_counter++;

            if (this.address_counter < 7)
            {
                // char
                this.CurrentAddress.Call += (char)(value & 0x7f);
            }
            else
            {
                // ssid etc.
                this.CurrentAddress.Ssid = (byte)(value & 0x78 >> 3);
                this.CurrentAddress.RR = (byte)(value & 0x6E >> 1);
                this.CurrentAddress.CRH = (byte)(value & 1);

                this.CurrentAddress.Call = this.CurrentAddress.Call.Trim();

                // reset
                this.address_counter = 0;
                this.Addresses.Add(this.CurrentAddress);
                this.CurrentAddress = new Ax25Address();
            }

            // return true if this is last address byte
            return (value & 0x80) == 0x80 && this.address_counter == 0;
        }

        public void AddControlByte(byte value)
        {
            this.ControlPf = (byte)((value & 0x10) >> 4);
            this.ControlNrmmm = (byte)((value & 0xE0) >> 5);

            if ((value & 1) == 0)
            {
                this.ControlType = Ax25ControlType.IShort;
                this.ControlNsssmm = (byte)((value & 0xE) >> 1);
            }
            else
            {
                this.ControlNsssmm = (byte)((value & 0xC) >> 2);
            }

            switch (value & 3)
            {
                case 1:
                    this.ControlType = Ax25ControlType.SShort;
                    break;
                case 3:
                    this.ControlType = Ax25ControlType.UShort;
                    break;
            }
        }

        public void AddPidByte(byte value)
        {

        }
    }
}