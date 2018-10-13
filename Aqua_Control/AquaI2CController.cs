﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
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

    public class AquaI2CController : IAquaI2CController
    {
        #region Members
        //Declared variables for I2C
        private const byte FDC1004_I2C_ADDR = 0x50;
        private const byte MEAS_ONE_CONTROL = 0x08;         // Address of the Measurement #1 register  
        private const byte MEAS_TWO_CONTROL = 0x09;         // Address of the Measurement #2 register    
        private const byte MEAS_THREE_CONTROL = 0x0A;       // Address of the Measurement #3 register 
        private const byte CIN1_CONTROL = 0x0D;             // Address of the Cin1 register
        private const byte FDC_CONF_CONTROL = 0x0C;         // Address of the FDC configuration register

        private Timer _timer;
        #endregion

        #region Properties
        public I2CDevice I2CSensor { get; private set; }
        public float FinalCapMeasure1 { get; private set; }
        public float FinalCapMeasure2 { get; private set; }
        public float FinalCapMeasure3 { get; private set; }
        public OneRegisterRead DataIn { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="AquaI2C"/>
        /// </summary>
        public AquaI2CController()
        {
            InitI2C();

            _timer = new Timer(I2CCheck, null, 0, 250);
        }

        public void InitI2C()
        {
            I2CSensor = Pi.I2C.AddDevice(FDC1004_I2C_ADDR);

            Console.WriteLine($"device added {I2CSensor.DeviceId}");

            // Write the register settings
            WriteToI2CDevice();
        }

        private void I2CCheck(object state)
        {
            double waterHeight = GetWaterHeight();
            Console.WriteLine($"{nameof(waterHeight)} = {waterHeight}");
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the height of the water in the tank
        /// </summary>
        /// <returns>A <see cref="double"/> representing the height</returns>
        public double GetWaterHeight()
        {
            //byte[] FDCCongAddrBuf = new byte[] { 0x0C };
            int Meas1AddrBufLSB = 0x00;
            int Meas1AddrBufMSB = 0x01;
            int Meas2AddrBufLSB = 0x02;
            int Meas2AddrBufMSB = 0x03;
            int Meas3AddrBufLSB = 0x04;
            int Meas3AddrBufMSB = 0x05;

            //DataIn = ReadOneReg(FDCCongAddrBuf);

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
            //byte[] WriteBuf_MeasurementOneFormat = new byte[] { MEAS_ONE_CONTROL, 0x1c, 0x00 };     // configs measurement 1 
            byte[] WriteBuf_MeasurementOneFormat = new byte[] {  0x1c, 0x00 };     // configs measurement 1 
            byte[] WriteBuf_MeasurementTwoFormat = new byte[] { 0x3c, 0x00 };     // configs measurement 2                         
            byte[] WriteBuf_MeasurementThreeFormat = new byte[] { 0x5c, 0x00 }; // configs measurement 3                        
            byte[] WriteBuf_Cin1 = new byte[] { 0x30, 0x00 };             // Set Offset for Cin1 to "6"pF based on datsheet calculations
            byte[] WriteBuf_FDC_Config = new byte[] { 0x0D, 0xE0 };   //set to read at 400S/s with repeat and read at measurement #1,#2,#3

            I2CSensor.WriteAddressWord(MEAS_ONE_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementOneFormat));
            I2CSensor.WriteAddressWord(MEAS_TWO_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementTwoFormat));
            I2CSensor.WriteAddressWord(MEAS_THREE_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementThreeFormat));
            I2CSensor.WriteAddressWord(CIN1_CONTROL, BitConverter.ToUInt16(WriteBuf_Cin1));
            I2CSensor.WriteAddressWord(FDC_CONF_CONTROL, BitConverter.ToUInt16(WriteBuf_FDC_Config));
        }

        private float ReadCapSen1_1(int RegAddrBuf1, int RegAddrBuf2)
        {
            int ReadBuf1;/* We read 2 bytes sequentially  */
            int ReadBuf2;/* We read 2 bytes sequentially  */

            ReadBuf1 = I2CSensor.ReadAddressWord(RegAddrBuf1);
            ReadBuf2 = I2CSensor.ReadAddressWord(RegAddrBuf2);
            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            CapMeasure capMeas1_1;
            capMeas1_1.CapReadLSB1_1 = (0xFF00 & ReadBuf1) >> 8;
            capMeas1_1.CapreadMSB1_1 = 0x00FF & ReadBuf1;

            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            capMeas1_1.CapReadLSB1_2 = (0xFF00 & ReadBuf2) >> 8;
            capMeas1_1.CapreadMSB1_2 = 0x00FF & ReadBuf2;

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
            ReadBuf = I2CSensor.Read(ReadBuf.Length);

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
