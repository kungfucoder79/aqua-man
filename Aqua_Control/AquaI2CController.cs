﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        private const int _Meas1AddrBufLSB = 0x00;
        private const int _Meas1AddrBufMSB = 0x01;
        private const int _Meas2AddrBufLSB = 0x02;
        private const int _Meas2AddrBufMSB = 0x03;
        private const int _Meas3AddrBufLSB = 0x04;
        private const int _Meas3AddrBufMSB = 0x05;


        private static float _InitCapMeasure2;

        private readonly Timer _timer;

        private double _TankHeight;
        private double _TankWidth;
        private double _TankDepth;

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
        public AquaI2CController(double tankWidth, double tankHeight, double tankDepth)
        {
            InitI2C();

            _timer = new Timer(I2CCheck, null, 0, 250);

            _TankHeight = tankHeight;
            _TankWidth = tankWidth;
            _TankDepth = tankDepth;
        }

        public void InitI2C()
        {
            I2CSensor = Pi.I2C.AddDevice(FDC1004_I2C_ADDR);

            Console.WriteLine($"device added {I2CSensor.DeviceId}");

            // Write the register settings
            WriteToI2CDevice();

            FinalCapMeasure2 = ReadCapSen1_1(_Meas2AddrBufLSB, _Meas2AddrBufMSB);
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

            //DataIn = ReadOneReg(FDCCongAddrBuf);
            Console.WriteLine($"{nameof(FinalCapMeasure1)}");
            FinalCapMeasure1 = ReadCapSen1_1(_Meas1AddrBufLSB, _Meas1AddrBufMSB);
            Console.WriteLine($"{nameof(FinalCapMeasure2)}");
            FinalCapMeasure2 = ReadCapSen1_1(_Meas2AddrBufLSB, _Meas2AddrBufMSB);
            Console.WriteLine($"{nameof(FinalCapMeasure3)}");
            FinalCapMeasure3 = ReadCapSen1_1(_Meas3AddrBufLSB, _Meas3AddrBufMSB);

            double WaterHeight = (0.4 * ((FinalCapMeasure2 - _InitCapMeasure2) / (FinalCapMeasure1 - FinalCapMeasure3)));
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
            byte[] WriteBuf_MeasurementOneFormat = new byte[] { 0x1c, 0x00 };     // configs measurement 1 
            byte[] WriteBuf_MeasurementTwoFormat = new byte[] { 0x3c, 0x00 };     // configs measurement 2                         
            byte[] WriteBuf_MeasurementThreeFormat = new byte[] { 0x5c, 0x00 }; // configs measurement 3                        
            //byte[] WriteBuf_Cin1 = new byte[] { 0x30, 0x00 };             // Set Offset for Cin1 to "6"pF based on datsheet calculations
            byte[] WriteBuf_FDC_Config = new byte[] { 0x0D, 0xE0 };   //set to read at 400S/s with repeat and read at measurement #1,#2,#3

            I2CSensor.WriteAddressWord(MEAS_ONE_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementOneFormat));
            I2CSensor.WriteAddressWord(MEAS_TWO_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementTwoFormat));
            I2CSensor.WriteAddressWord(MEAS_THREE_CONTROL, BitConverter.ToUInt16(WriteBuf_MeasurementThreeFormat));
            //I2CSensor.WriteAddressWord(CIN1_CONTROL, BitConverter.ToUInt16(WriteBuf_Cin1));
            I2CSensor.WriteAddressWord(FDC_CONF_CONTROL, BitConverter.ToUInt16(WriteBuf_FDC_Config));
        }

        private float ReadCapSen1_1(int RegAddrBuf1, int RegAddrBuf2)
        {
            int temp1 = (I2CSensor.ReadAddressWord(RegAddrBuf1));
            int temp2 = (I2CSensor.ReadAddressWord(RegAddrBuf2));

            byte[] byteData1 = BitConverter.GetBytes(temp1);
            byte[] byteData2 = BitConverter.GetBytes(temp2);
            byte[] byteData3 = { byteData2[0], byteData1[1], byteData1[0], 0 };

            int CapLabel1Both = BitConverter.ToInt32(byteData3);

            float FinalCapMeasure = CapLabel1Both / 524288f;

            Console.WriteLine($"{nameof(byteData3)} = {Convert.ToString(byteData3[0], 2)} {Convert.ToString(byteData3[1], 2)} {Convert.ToString(byteData3[2], 2)} {Convert.ToString(byteData3[3], 2)}");
            Console.WriteLine($"CapLabel1Both = {CapLabel1Both}");
            Console.WriteLine($"FinalCapMeasure = {FinalCapMeasure.ToString(".0###########")}");
            Console.WriteLine();
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

        public void CalabrateSensor()
        {
            _InitCapMeasure2 = ReadCapSen1_1(_Meas2AddrBufLSB, _Meas2AddrBufMSB);
        }

        public void UpdateTankSpecs(double tankWidth, double tankHeight, double tankDepth)
        {
            _TankHeight = tankHeight;
            _TankWidth = tankWidth;
            _TankDepth = tankDepth;
        }
        #endregion
    }
}
