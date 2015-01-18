
using NtImageProcessor.MetaData.Structure;
using NtPhotoInfoEditor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace NtPhotoInfoEditor.DataModel
{
    public class PhotoPlaybackData
    {
        public PhotoPlaybackData() { }
        private ObservableCollection<EntryViewData> _EntryList = new ObservableCollection<EntryViewData>();
        public ObservableCollection<EntryViewData> EntryList
        {
            get { return _EntryList; }
            set
            {
                _EntryList = value;
            }
        }

        private JpegMetaData _MetaData;
        public JpegMetaData MetaData
        {
            get { return _MetaData; }
            set
            {
                _MetaData = value;
                if (value == null)
                {
                    ShowInvalidData();
                }
                else
                {
                    UpdateEntryList(value);
                    if (EntryList.Count == 0)
                    {
                        ShowInvalidData();
                    }
                }
            }
        }

        Dictionary<string, uint> SupportedMetadataItems = new Dictionary<string, uint>()
        {
            {"Fnumber", 0x829D},
            {"ExposureTime", 0x829A},
            {"Iso", 0x8827},
            {"FocalLength", 0x920A},
            {"CameraModel", 0x0110},
            {"CameraMaker", 0x010F},
            {"ImageWidth", 0xA002},
            {"ImageHeight", 0xA003},
            {"DateTime", 0x9003},
            {"ExposureProgram", 0x8822},
            {"WhiteBalanceMode", 0xA403},
            {"WhiteBalanceDetailType", 0x9208},
            {"DocumentName", 0x010D},
            {"ExposureCompensation", 0x9204},
            {"MeteringMode", 0x9207},
            {"Flash", 0x9209},
            {"LensModel", 0xA434},
            {"GpsLatitude", 0x01},
        };

        void ShowInvalidData()
        {
            EntryList.Clear();
            EntryList.Add(new EntryViewData() { Name = "", ValuesList = new List<string>() { "" } });
        }

        private void UpdateEntryList(JpegMetaData metadata)
        {
            EntryList.Clear();

            var converter = new MetaDataValueConverter();
            var type = converter.GetType().GetTypeInfo();

            foreach (var name in SupportedMetadataItems.Keys)
            {
                var key = SupportedMetadataItems[name];
                var EntryName = MetaDataValueConverter.MetaDataEntryName(name);
                var ValueName = "";
                var entry = FindFirstEntry(metadata, key);
                if (entry == null) { continue; }

                var EntryData = AsSpecialEntry(metadata, name);
                if (EntryData != null)
                {
                    EntryList.Add(EntryData);
                    continue;
                }

                var method = type.GetDeclaredMethod(name + "Name");
                if (method == null)
                {
                    Logger.Log("Can't find method: " + name + "Name");
                }

                switch (entry.Type)
                {
                    case Entry.EntryType.Long:
                    case Entry.EntryType.Short:
                        ValueName = (string)method.Invoke(converter, new object[] { entry.UIntValues });
                        break;
                    case Entry.EntryType.SLong:
                    case Entry.EntryType.SShort:
                        ValueName = (string)method.Invoke(converter, new object[] { entry.SIntValues });
                        break;
                    case Entry.EntryType.Rational:
                        ValueName = (string)method.Invoke(converter, new object[] { entry.UFractionValues });
                        break;
                    case Entry.EntryType.SRational:
                        ValueName = (string)method.Invoke(converter, new object[] { entry.SFractionValues });
                        break;
                    default:
                        ValueName = (string)method.Invoke(converter, new object[] { GetStringValue(metadata, key) });
                        break;
                }

                EntryList.Add(CreateEntry(EntryName, ValueName, key));
            }
        }

        EntryViewData AsSpecialEntry(JpegMetaData metadata, string name)
        {
            if (!SupportedMetadataItems.ContainsKey(name)) { return null; }

            var key = SupportedMetadataItems[name];

            switch (name)
            {
                case "GpsLatitude":
                    var geotagEntry = FindFirstEntry(metadata, key);
                    if (geotagEntry != null)
                    {
                        return CreateEntry(SystemUtil.GetStringResource("MetaData_Geotag"),
                            MetaDataValueConverter.Geoinfo(metadata.GpsIfd),
                            key);
                    }
                    break;
            }
            return null;
        }

        EntryViewData CreateEntry(string name, string value, uint key)
        {
            Logger.Log(name + " " + value + " " + key);
            return new EntryViewData()
            {
                Name = name,
                ValuesList = new List<string>() { value },
                MetadataKey = key,
            };
        }

        EntryViewData CreateEntry(string name, List<string> value, uint key)
        {
            return new EntryViewData()
            {
                Name = name,
                ValuesList = value,
                MetadataKey = key,
            };
        }

        string GetStringValue(JpegMetaData metadata, uint key)
        {
            var entry = FindFirstEntry(metadata, key);
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

        Entry FindFirstEntry(JpegMetaData metadata, uint key)
        {
            if (metadata == null) { return null; }

            if (metadata.PrimaryIfd != null && metadata.PrimaryIfd.Entries.ContainsKey(key))
            {
                return metadata.PrimaryIfd.Entries[key];
            }
            else if (metadata.ExifIfd != null && metadata.ExifIfd.Entries.ContainsKey(key))
            {
                return metadata.ExifIfd.Entries[key];
            }
            else if (metadata.GpsIfd != null && metadata.GpsIfd.Entries.ContainsKey(key))
            {
                return metadata.GpsIfd.Entries[key];
            }
            return null;
        }
    }

    public class EntryViewData
    {
        public string Name { get; set; }
        public List<string> ValuesList { get; set; }
        public uint MetadataKey { get; set; }
    }

}
