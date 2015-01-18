﻿
using NtImageProcessor.MetaData.Structure;
using NtPhotoInfoEditor.DataModel;
using System.Collections.Generic;

namespace NtPhotoInfoEditor.Utils
{
    public class MetaDataValueConverter
    {
        public static string MetaDataEntryName(string key)
        {
            var name = SystemUtil.GetStringResource("MetaDataName_" + key);
            if (name == "") { return key; }
            return name;
        }

        private static Dictionary<uint, string> ExposureProgramNames = new Dictionary<uint, string>()
        {
            {0x0, SystemUtil.GetStringResource("ExifExposureProgram_Unknown")},
            {0x1, SystemUtil.GetStringResource("ExifExposureProgram_M")},
            {0x2, SystemUtil.GetStringResource("ExifExposureProgram_P")},
            {0x3, SystemUtil.GetStringResource("ExifExposureProgram_A")},
            {0x4, SystemUtil.GetStringResource("ExifExposureProgram_S")},
            {0x5, SystemUtil.GetStringResource("ExifExposureProgram_SlowSpeed")},
            {0x6, SystemUtil.GetStringResource("ExifExposureProgram_HighSpeed")},
            {0x7, SystemUtil.GetStringResource("ExifExposureProgram_Portrait")},
            {0x8, SystemUtil.GetStringResource("ExifExposureProgram_Landscape")},
            {0x9, SystemUtil.GetStringResource("ExifExposureProgram_Bulb")},
        };

        private static Dictionary<uint, string> WhitebalanceNames = new Dictionary<uint, string>()
        {
            { 0, SystemUtil.GetStringResource("ExifWBValue_Unknown")},
            { 1, SystemUtil.GetStringResource("ExifWBValue_Daylight")},
            { 2, SystemUtil.GetStringResource("ExifWBValue_Fluorescent")},
            { 3, SystemUtil.GetStringResource("ExifWBValue_TungstenIncandescent")},
            { 4, SystemUtil.GetStringResource("ExifWBValue_Flash")},
            { 9, SystemUtil.GetStringResource("ExifWBValue_FineWeather")},
            { 10, SystemUtil.GetStringResource("ExifWBValue_Cloudy")},
            { 11, SystemUtil.GetStringResource("ExifWBValue_Shade")},
            { 12, SystemUtil.GetStringResource("ExifWBValue_DaylightFluorescent")},
            { 13, SystemUtil.GetStringResource("ExifWBValue_DayWhiteFluorescent")},
            { 14, SystemUtil.GetStringResource("ExifWBValue_CoolWhiteFluorescent")},
            { 15, SystemUtil.GetStringResource("ExifWBValue_WhiteFluorescent")},
            { 16, SystemUtil.GetStringResource("ExifWBValue_WarmWhiteFluorescent")},
            { 17, SystemUtil.GetStringResource("ExifWBValue_StandardLightA")},
            { 18, SystemUtil.GetStringResource("ExifWBValue_StandardLightB")},
            { 19, SystemUtil.GetStringResource("ExifWBValue_StandardLightC")},
            { 20, SystemUtil.GetStringResource("ExifWBValue_D55")},
            { 21, SystemUtil.GetStringResource("ExifWBValue_D65")},
            { 22, SystemUtil.GetStringResource("ExifWBValue_D75")},
            { 23, SystemUtil.GetStringResource("ExifWBValue_D50")},
            { 24, SystemUtil.GetStringResource("ExifWBValue_ISOStudioTungsten")},
            { 255, SystemUtil.GetStringResource("ExifWBValue_Other")},
        };

        private static Dictionary<uint, string> MeteringModeNames = new Dictionary<uint, string>(){
            {0, SystemUtil.GetStringResource("Exif_MeteringMode_Unknown")},
            {1, SystemUtil.GetStringResource("Exif_MeteringMode_Average")},
            {2, SystemUtil.GetStringResource("Exif_MeteringMode_CenterWeightedAverage")},
            {3, SystemUtil.GetStringResource("Exif_MeteringMode_Spot")},
            {4, SystemUtil.GetStringResource("Exif_MeteringMode_MultiSpot")},
            {5, SystemUtil.GetStringResource("Exif_MeteringMode_MultiSegment")},
            {6, SystemUtil.GetStringResource("Exif_MeteringMode_Partial")},
            {255, SystemUtil.GetStringResource("Exif_MeteringMode_Other")},
        };

        public static List<string> Geoinfo(IfdData GpsIfd)
        {
            var values = new List<string>();
            // lat
            if (GpsIfd.Entries.ContainsKey(0x1) && GpsIfd.Entries.ContainsKey(0x2) && GpsIfd.Entries[0x2].Count >= 3)
            {
                var entry = GpsIfd.Entries[0x2];
                values.Add(GetStringValue(GpsIfd.Entries[0x1]) + " " + entry.DoubleValues[0] + "°" + entry.DoubleValues[1] + "'" + entry.DoubleValues[2] + "''");
            }

            if (GpsIfd.Entries.ContainsKey(0x3) && GpsIfd.Entries.ContainsKey(0x4) && GpsIfd.Entries[0x4].Count >= 3)
            {
                var entry = GpsIfd.Entries[0x4];
                values.Add(GetStringValue(GpsIfd.Entries[0x3]) + " " + entry.DoubleValues[0] + "°" + entry.DoubleValues[1] + "'" + entry.DoubleValues[2] + "''");
            }

            if (GpsIfd.Entries.ContainsKey(0x12))
            {
                values.Add(GetStringValue(GpsIfd.Entries[0x12]));
            }

            if (GpsIfd.Entries.ContainsKey(0x1D))
            {
                values.Add(GetStringValue(GpsIfd.Entries[0x1D]));
            }

            return values;
        }

        public static string FnumberName(UnsignedFraction[] value)
        {
            return (value[0].Numerator / value[0].Denominator).ToString("F1");
        }

        public static string ExposureTimeName(UnsignedFraction[] value)
        {
            var numerator = value[0].Numerator;
            var denominator = value[0].Denominator;

            if (numerator == 1)
            {
                return numerator + "/" + denominator + SystemUtil.GetStringResource("Seconds");
            }
            else if (denominator == 1)
            {
                return numerator + SystemUtil.GetStringResource("Seconds");
            }

            // difficult cases,,
            if (numerator > denominator)
            {
                // longer than 1 sec.
                double val = numerator / denominator;
                return val.ToString() + SystemUtil.GetStringResource("Seconds");
            }

            // reduction forcibly
            int newDenominator = (int)((double)denominator / (double)numerator);
            return "1/" + newDenominator + SystemUtil.GetStringResource("Seconds");
        }

        public static string IsoName(uint[] value)
        {
            return value[0].ToString();
        }

        public static string FocalLengthName(UnsignedFraction[] value)
        {
            return (value[0].Numerator / value[0].Denominator).ToString() + "mm";
        }

        public static string CameraModelName(string value)
        {
            return value;
        }

        public static string CameraMakerName(string value)
        {
            return value;
        }

        public static string ImageWidthName(uint[] value)
        {
            return value[0].ToString() + "px";
        }

        public static string ImageHeightName(uint[] value)
        {
            return value[0].ToString() + "px";
        }

        public static string DateTimeName(string value)
        {
            return value;
        }

        public static string ExposureProgramName(uint[] value)
        {
            var key = value[0];
            if (ExposureProgramNames.ContainsKey(key))
            {
                return ExposureProgramNames[key];
            }
            return key.ToString("X2");
        }

        public static string WhiteBalanceModeName(uint[] value)
        {
            if (value[0] == 0x0)
            {
                return SystemUtil.GetStringResource("WB_Auto");
            }
            return "See Detail.";
        }

        public static string WhiteBalanceDetailTypeName(uint[] value)
        {
            if (WhitebalanceNames.ContainsKey(value[0]))
            {
                return WhitebalanceNames[value[0]];
            }
            return value.ToString();
        }

        public static string DocumentNameName(string value)
        {
            return value;
        }

        public static string ExposureCompensationName(SignedFraction[] v)
        {
            var value = v[0].Numerator / v[0].Denominator;
            if (value > 0)
            {
                return "+" + value + "EV";
            }
            return value + "EV";
        }

        public string MeteringModeName(uint[] value)
        {
            if (MeteringModeNames.ContainsKey(value[0]))
            {
                return MeteringModeNames[value[0]];
            }
            return SystemUtil.GetStringResource("Exif_MeteringMode_Unknown");
        }

        public static string FlashName(uint[] value)
        {
            switch (value[0])
            {
                case 0x0:
                    return SystemUtil.GetStringResource("Exif_Flash_NoFlash");
                case 0x01:
                case 0x05:
                case 0x07:
                    return SystemUtil.GetStringResource("Exif_Flash_Fired");
                case 0x08:
                    return SystemUtil.GetStringResource("Exif_Flash_On_NotFired");
                case 0x09:
                case 0x0d:
                case 0x0f:
                    return SystemUtil.GetStringResource("Exif_Flash_On_Fired");
                case 0x10:
                case 0x14:
                    return SystemUtil.GetStringResource("Exif_Flash_Off_NotFired");
                case 0x18:
                    return SystemUtil.GetStringResource("Exif_Flash_Auto_NotFired");
                case 0x19:
                case 0x1d:
                case 0x1f:
                    return SystemUtil.GetStringResource("Exif_Flash_Auto_Fired");
                case 0x20:
                    return SystemUtil.GetStringResource("Exif_Flash_NoFlashFunction");
                case 0x30:
                    return SystemUtil.GetStringResource("Exif_Flash_NoFlashFunction");
                case 0x41:
                case 0x45:
                case 0x47:
                    return SystemUtil.GetStringResource("Exif_Flash_Fired_RedEyeReduction");
                case 0x49:
                case 0x4d:
                case 0x4f:
                    return SystemUtil.GetStringResource("Exif_Flash_On_Fired_RedEyeReduction");
                case 0x50:
                    return SystemUtil.GetStringResource("Exif_Flash_Off_NotFired_RedEyeReduction");
                case 0x58:
                    return SystemUtil.GetStringResource("Exif_Flash_Auto_NotFired_RedEyeReduction");
                case 0x59:
                case 0x5d:
                case 0x5f:
                    return SystemUtil.GetStringResource("Exif_Flash_Auto_Fired_RedEyeReduction");
                default:
                    return SystemUtil.GetStringResource("Exif_Flash_Unknown");
            }
        }

        public static string LensModelName(string value)
        {
            return value;
        }

        static string GetStringValue(Entry entry)
        {
            if (entry == null) { return "null"; }
            switch (entry.Type)
            {
                case Entry.EntryType.Ascii:
                    return entry.StringValue;
                case Entry.EntryType.Byte:
                    return entry.value.ToString();
                case Entry.EntryType.Long:
                case Entry.EntryType.Short:
                    return entry.UIntValues[0].ToString();
                case Entry.EntryType.SLong:
                case Entry.EntryType.SShort:
                    return entry.SIntValues[0].ToString();
                case Entry.EntryType.Rational:
                case Entry.EntryType.SRational:
                    return entry.DoubleValues[0].ToString();
            }
            return "--";
        }
    }
}
