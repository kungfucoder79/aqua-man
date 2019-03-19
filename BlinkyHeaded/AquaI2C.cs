using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Aqua_ControlUWP
{
    public struct OneRegisterRead
    {
        public int DataMSB;
        public int DataLSB;
    };

    struct CapMeasure
    {
        public int CapReadLSB1_1;
        public int CapreadMSB1_1;
        public int CapReadLSB1_2;
        public int CapreadMSB1_2;
    };

    /// <summary>
    /// Class for communicating with the I2C protocol
    /// </summary>
    public class AquaI2C
    {
        #region ctor
        /// <summary>
        /// Creates a new <see cref="AquaI2C"/>
        /// </summary>
        public AquaI2C()
        {
            InitI2C();
        }
        #endregion

        #region Properties
        public I2cDevice I2CSensor { get; private set; }
        public float FinalCapMeasure1 { get; private set; }
        public float FinalCapMeasure2 { get; private set; }
        public float FinalCapMeasure3 { get; private set; }
        public OneRegisterRead DataIn { get; private set; }
        #endregion

        #region Members
        //Declared variables for I2C
        private const byte FDC1004_I2C_ADDR = 0x50;
        private const byte MEAS_ONE_CONTROL = 0x08;         // Address of the Measurement #1 register  
        private const byte MEAS_TWO_CONTROL = 0x09;         // Address of the Measurement #2 register    
        private const byte MEAS_THREE_CONTROL = 0x0A;       // Address of the Measurement #3 register 
        private const byte CIN1_CONTROL = 0x0D;             // Address of the Cin1 register
        private const byte FDC_CONF_CONTROL = 0x0C;         // Address of the FDC configuration register
        #endregion

        #region Methods

        /// <summary>
        /// Gets the height of the water in the tank
        /// </summary>
        /// <returns>A <see cref="double"/> representing the height</returns>
        public double GetWaterHeight()
        {
            byte[] FDCCongAddrBuf = new byte[] { 0x0C };
            byte[] Meas1AddrBufLSB = new byte[] { 0x00 };
            byte[] Meas1AddrBufMSB = new byte[] { 0x01 };
            byte[] Meas2AddrBufLSB = new byte[] { 0x02 };
            byte[] Meas2AddrBufMSB = new byte[] { 0x03 };
            byte[] Meas3AddrBufLSB = new byte[] { 0x04 };
            byte[] Meas3AddrBufMSB = new byte[] { 0x05 };

            DataIn = ReadOneReg(FDCCongAddrBuf);

            FinalCapMeasure1 = ReadCapSen1_1(Meas1AddrBufLSB, Meas1AddrBufMSB);
            FinalCapMeasure2 = ReadCapSen1_1(Meas2AddrBufLSB, Meas2AddrBufMSB);
            FinalCapMeasure3 = ReadCapSen1_1(Meas3AddrBufLSB, Meas3AddrBufMSB);

            double WaterHeight = (5 * ((FinalCapMeasure2 - 1.80) / (FinalCapMeasure1 - FinalCapMeasure3)));
            return WaterHeight;
        }

        /// <summary>
        /// Resets the I2C port
        /// </summary>
        public void Reset()
        {
            byte[] WriteBuf_FDC_Config = new byte[] { FDC_CONF_CONTROL, 0x8C, 0x00 };
            I2CSensor.Write(WriteBuf_FDC_Config);
            WriteToI2CDevice();
        }

        private async void InitI2C()
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(FDC1004_I2C_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;                       // 400KHz bus speed 
            I2cController controller = await Windows.Devices.I2c.I2cController.GetDefaultAsync();
            I2CSensor = controller.GetDevice(settings);    // Create an I2cDevice with our selected bus controller and I2C settings

            // Write the register settings
            WriteToI2CDevice();
        }

        private void WriteToI2CDevice()
        {
            /* 
             * Initialize the FDC1004 capacitance sensor:
             *
             * For this device, we create 3-byte write buffers: ==> this device uses 
             * The first byte is the register address we want to write to.
             * The second byte is the contents of the MSB that we want to write to the register. 
             * The third byte is the contents of the LSB that we want to write to the register.
             */
            byte[] WriteBuf_MeasurementOneFormat = new byte[] { MEAS_ONE_CONTROL, 0x1c, 0x00 };     // configs measurement 1 
            byte[] WriteBuf_MeasurementTwoFormat = new byte[] { MEAS_TWO_CONTROL, 0x3c, 0x00 };     // configs measurement 2                         
            byte[] WriteBuf_MeasurementThreeFormat = new byte[] { MEAS_THREE_CONTROL, 0x5c, 0x00 }; // configs measurement 3                        
            byte[] WriteBuf_Cin1 = new byte[] { CIN1_CONTROL, 0x30, 0x00 };             // Set Offset for Cin1 to "6"pF based on datsheet calculations
            byte[] WriteBuf_FDC_Config = new byte[] { FDC_CONF_CONTROL, 0x0D, 0xE0 };   //set to read at 400S/s with repeat and read at measurement #1,#2,#3

            I2CSensor.Write(WriteBuf_MeasurementOneFormat);
            I2CSensor.Write(WriteBuf_MeasurementTwoFormat);
            I2CSensor.Write(WriteBuf_MeasurementThreeFormat);
            I2CSensor.Write(WriteBuf_Cin1);
            I2CSensor.Write(WriteBuf_FDC_Config);
        }

        private float ReadCapSen1_1(byte[] RegAddrBuf1, byte[] RegAddrBuf2)
        {
            byte[] ReadBuf1, ReadBuf2;

            ReadBuf1 = new byte[2];  /* We read 2 bytes sequentially  */
            ReadBuf2 = new byte[2];  /* We read 2 bytes sequentially  */
            I2CSensor.WriteRead(RegAddrBuf1, ReadBuf1);

            I2CSensor.WriteRead(RegAddrBuf2, ReadBuf2);
            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            int RawData = BitConverter.ToInt16(ReadBuf1, 0);
            //int RawData = BitConverter.ToInt16(ReadBuf, 0);
            CapMeasure capMeas1_1;
            capMeas1_1.CapReadLSB1_1 = (0xFF00 & RawData) >> 8;
            capMeas1_1.CapreadMSB1_1 = 0x00FF & RawData;

            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            RawData = BitConverter.ToInt16(ReadBuf2, 0);
            capMeas1_1.CapReadLSB1_2 = (0xFF00 & RawData) >> 8;
            capMeas1_1.CapreadMSB1_2 = 0x00FF & RawData;

            int CapLabel1_1 = (capMeas1_1.CapreadMSB1_1 * 256 + capMeas1_1.CapReadLSB1_1);
            int CapLabel1_2 = capMeas1_1.CapreadMSB1_2 * 256 + capMeas1_1.CapReadLSB1_2;
            int CapLabel1Both = (CapLabel1_1 * 256 + capMeas1_1.CapreadMSB1_2);
            float FinalCapMeasure = CapLabel1Both / 524288;

            return FinalCapMeasure;
        }

        private OneRegisterRead ReadOneReg(byte[] RegAddrBuf)
        {
            byte[] ReadBuf;
            ReadBuf = new byte[2];  // need to read the two bytes stored in the targeted register
            I2CSensor.WriteRead(RegAddrBuf, ReadBuf);

            // A little confusing but decided to parse the two bytes into separate bytes, convert to 'int'
            // then return this information so we can perform calculations.
            int RawData = BitConverter.ToInt16(ReadBuf, 0);

            OneRegisterRead OneRegReadDataOut;
            OneRegReadDataOut.DataMSB = 0xFF00 & RawData >> 8;
            OneRegReadDataOut.DataLSB = 0x00FF & RawData; // if reading Done bit, use this statement ==> ((byte)(0x08 & RawData) >> 3)

            return OneRegReadDataOut;
        }
        #endregion
    }
}
